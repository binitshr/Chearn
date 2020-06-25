var courseID = document.getElementById('CourseID').value


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
var generateGraph = async function () {
    var lessons = await axios.get(`/Lessons/GetStudentLessons/?courseID=${courseID}`)
    var edges = await axios.get(`/Edge/GetAllEdges/?courseID=${courseID}`)
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
                source: "lesson" + edge.ParentID, target: "lesson" + edge.ChildID
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
                'height': '5em',
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

  


   

}
generateGraph()

//builds cytoscape logic

//initialize extensions, loads nodes



