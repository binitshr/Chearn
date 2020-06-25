var courseID = document.getElementById('CourseID').value

const searchBar = document.getElementById('lessonSearchbar')
const resultRow = document.getElementById('resultRow')


var refreshedRow = resultRow.cloneNode(false)
var generateIcons = function (id, description) {
    return `<hr/>
            <button class='cn-icon'>
                <a href="${window.location.origin}/Review/lessonPage/?lessonID=${id}" title="${description}" data-toggle="popover" data-trigger="hover" data-content="Placeholder for the description of the lesson" }>
                    Take Lesson
                </a>
            </button>`
}

var searchEvent = async function () {
    var lessonList = await axios.get(`/Lessons/GetLessonsByName/?courseID=${courseID}&name=${searchBar.value}`)
    if (lessonList.status == 200) {
        resultRow.innerHTML = ''
        resultRow.appendChild(searchBar)
        searchBar.focus()
        for (var lesson of lessonList.data) {

            //clears resultRow's child nodes
            var newLesson = document.createElement('div')
            var cardBody = document.createElement('div')
            var cardText = document.createElement('div')
            newLesson.className = 'card lesson'


            //TODO: abstract logic into card builder function
            newLesson.id = 'lesson' + lesson.ID
            cardBody.className = 'card-body'
            cardText.className = 'card-text'
            cardText.style = 'text-align: center'
            cardText.innerHTML = lesson.Title + generateIcons(lesson.ID, lesson.Description)

            cardBody.appendChild(cardText)
            newLesson.appendChild(cardBody)
            resultRow.appendChild(newLesson)
        }
    }
    else {
        console.error("Unhandled error in NodeGraph.js line 21")
    }


}



searchEvent();
searchBar.addEventListener('input', searchEvent) 