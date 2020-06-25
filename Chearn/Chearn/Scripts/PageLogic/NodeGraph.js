const courseID = document.getElementById('CourseID').value
const searchBar = document.getElementById('lessonSearchbar')
const resultRow = document.getElementById('resultRow')

const graphDiv = document.getElementById('cy')
const bgColorPrimary = getComputedStyle(document.documentElement)
    .getPropertyValue('--primary-highlight-color');

const textColorPrimary = getComputedStyle(document.documentElement)
    .getPropertyValue('--primary-text-color')
const highlightColor = getComputedStyle(document.documentElement)
    .getPropertyValue('--secondary-highlight-color')

var edgelessSelectedNodes = []
var edgesToSave = []
var removedEdgesToSave = []

//utility functions
var MouseInBoundingRect = (mouseX, mouseY, rect) =>
        !((mouseX - rect.left < 0) || (mouseX - rect.left) > rect.width
        || (mouseY - rect.top) < 0 || (mouseY - rect.top) > rect.height)
 
var formatNodeTitle = function (NodeTitle) {
    const NodeTitleMaxCharacters = 40;
    const CharactersPerLine = 15;
    const MaxDistanceToNextSpace = 5;
    //todo: remove jquery if possible
    let formattedNodeTitle = NodeTitle.slice(0, CharactersPerLine);
    if (NodeTitle.length > CharactersPerLine)
        for (var i = CharactersPerLine; i < NodeTitleMaxCharacters; i += CharactersPerLine) {
            var indexOfNextSpace = NodeTitle.indexOf(' ', i)
            if (indexOfNextSpace >= 0 && indexOfNextSpace - i < MaxDistanceToNextSpace) {
                formattedNodeTitle += NodeTitle.slice(i, indexOfNextSpace) + '\n'
                //resets index to one after last offset
                i = indexOfNextSpace + 1;
            }
            else {
                formattedNodeTitle += NodeTitle.slice(i - CharactersPerLine, i) + '\n'
            }
        }

    if (NodeTitle.length > NodeTitleMaxCharacters)
        formattedNodeTitle = formattedNodeTitle.substring(0, formattedNodeTitle.length - 2) + '...'
    return formattedNodeTitle
}
var cy = null
var loadedNodes = []
var loadedEdges = []
//generates graph on load
var generateGraph =  async function () {
    var lessons =  await axios.get(`/Lessons/GetAllConnectedLessons/?courseID=${courseID}`)
    var edges =  await axios.get(`/Edge/GetAllEdges/?courseID=${courseID}`)
    for (var lesson of lessons.data) {
        loadedNodes.push({
            data: { id: "lesson" + lesson.ID, display: formatNodeTitle(lesson.Title) },
        })
        edgelessSelectedNodes.push(lesson.Title)
    }
    for (var edge of edges.data) {
        loadedEdges.push({
            data: {
                id: edge.ParentID + ',' + edge.ChildID,
                source: "lesson" + edge.ParentID, target: "lesson"+ edge.ChildID
            },
        })
    }
    cy = cytoscape({
        container: document.getElementById('cy'),
        wheelSensitivity: 0.2,
        elements: {
            nodes: loadedNodes,
            edges: loadedEdges
        },
        style: [{
            selector: 'node',
            css: {
                'label': `data(display)`,
                'shape': 'roundrectangle',
                'color': textColorPrimary,
                'background-color': bgColorPrimary,
                'font-size': '.75em',
                'text-valign': 'center',
                'text-halign': 'center',
                'text-wrap': 'wrap',
                'height': '3em',
                'width': '7em',
                "text-background-opacity": 1,
                "text-background-color": bgColorPrimary,
            }
        },
        {
            selector: 'edge',
            style: {
                'curve-style': 'bezier',
                'target-arrow-shape': 'triangle'
            }
        },

        // some style for the extension

        {
            selector: '.eh-handle',
            style: {
                'background-color': highlightColor,
                'width': 12,
                'height': 12,
                'shape': 'ellipse',
                'overlay-opacity': 0,
                'border-width': 10, // makes the handle easier to hit
                'border-opacity': 0
            }
        },

        {
            selector: '.eh-hover',
            style: {
                'background-color': highlightColor
            }
        },

        {
            selector: '.eh-source',
            style: {
                'border-width': 2,
                'border-color': highlightColor
            }
        },

        {
            selector: '.eh-target',
            style: {
                'border-width': 1,
                'border-color': highlightColor
            }
        },

        {
            selector: '.eh-preview, .eh-ghost-edge',
            style: {
                'background-color': highlightColor,
                'line-color': highlightColor,
                'target-arrow-color': highlightColor,
                'source-arrow-color': highlightColor
            }
        },

        {
            selector: '.eh-ghost-edge.eh-preview-active',
            style: {
                'opacity': 0
            }
        }
        ]
        })
        cy.edgehandles();
    cy.on('ehcomplete', function (event, sourceNode, targetNode, addedEles) {
        //adds unique edges to edgesToSave
        const sourceID = sourceNode._private.data.id
        const targetID = targetNode._private.data.id
        const edge = { child: targetID, parent: sourceID }
        if (edgesToSave.filter(e => e.parent == edge.parent && e.child == edge.child).length < 1) {
            edgesToSave.push(edge)
        }
    })

    //handles delete edge event
    document.addEventListener('keydown', function () {

        //TODO: check remove functionality after load
        if (event.key == "Delete" && cy.$('edge:selected').length > 0) {
            var edgeData = cy.$('edge:selected')._private.map.entries().next().value[1].ele._private.data;
            var edge = { parent: edgeData.target, child: edgeData.source }
            if (edgesToSave.filter(e => e.parent == edge.parent && e.child == edge.child).length == 0) {
                removedEdgesToSave.push(edge)
            }

            cy.remove(cy.$('edge:selected'))
        }
    })
    //draggable logic
    var dragElement = null
    var dropElement = null
    const handleDropZone = function (event) {
        event.preventDefault()
        if (!dropElement || event.target.id != dropElement.id) {
            dropElement = event.target
        }
    }
    graphDiv.addEventListener('dragover', handleDropZone)


    //handles searchbar logic
    var refreshedRow = resultRow.cloneNode(false);
    var searchEvent = async function () {
        var lessonList = await axios.get(`/Lessons/GetLessonsByName/?courseID=${courseID}&name=${searchBar.value}`)
        if (lessonList.status == 200) {
            resultRow.innerHTML = ''
            resultRow.appendChild(searchBar)
            searchBar.focus()
            for (var lesson of lessonList.data) {

                var formatNode = formatNodeTitle
                var cyy = cy;
                //clears resultRow's child nodes
                var newLesson = document.createElement('div')
                var cardBody = document.createElement('div')
                var cardText = document.createElement('div')

                //greys out nodes currently in graph
                if (edgelessSelectedNodes.includes(lesson.Title)) {
                    newLesson.className = 'card new-lesson greyed-out'
                    newLesson.setAttribute('draggable', false)

                }
                else {
                    newLesson.className = 'card new-lesson'
                    newLesson.setAttribute('draggable', true)
                    newLesson.addEventListener('dragstart', function () {
                        dragElement = event.target;
                    })
                    newLesson.addEventListener('dragend', function () {
                        let canvas = $("#cy").find("canvas").get(0);
                        if (MouseInBoundingRect(event.clientX, event.clientY, canvas.getBoundingClientRect())) {
                            const NodeTitle = dragElement.children[0].children[0].innerText
                            this.className = 'card new-lesson greyed-out'
                            edgelessSelectedNodes.push(NodeTitle)
                            var formattedNodeTitle = formatNode(NodeTitle)

                            cyy.add({
                                group: 'nodes',
                                data: { id: dragElement.id, display: formattedNodeTitle },
                                renderedPosition: {
                                    x: event.pageX - $(canvas).offset().left,
                                    y: event.pageY - $(canvas).offset().top
                                }

                            })
                        }

                    })
                }


                //TODO: abstract logic into card builder function
                newLesson.id = 'lesson' + lesson.ID
                cardBody.className = 'card-body'
                cardText.className = 'card-text'
                cardText.style = 'text-align: center'
                cardText.innerText = lesson.Title

                cardBody.appendChild(cardText)
                newLesson.appendChild(cardBody)
                resultRow.appendChild(newLesson)
            }
        }
        else {
            console.error("Unhandled error in NodeGraph.js line 21")
        }
    }

    //handles save logic
    document.getElementById('saveButton').addEventListener('click', function () {
        event.preventDefault()
        //axios.get(`/Edge/HandleEdgeSave/?courseID=${courseID}&edgesToAdd=null&edgesToRemove=null`)

        var response = axios.post(`/Edge/HandleEdgeSave/?courseID=${courseID}&edgesToAddJson=${JSON.stringify(edgesToSave)}&edgesToRemoveJson=${JSON.stringify(removedEdgesToSave)}`)
    })

    //handles revert logic
    document.getElementById('revertButton').addEventListener('click', function () {
        location.reload()
    })

    //handles reset logic
    document.getElementById('resetButton').addEventListener('click', function () {
        console.log('TODO: add reset functionality')
    })
    searchEvent = _.debounce(searchEvent, 100)
    searchEvent();
    searchBar.addEventListener('input', searchEvent)

}
generateGraph()

//builds cytoscape logic

//initialize extensions, loads nodes



