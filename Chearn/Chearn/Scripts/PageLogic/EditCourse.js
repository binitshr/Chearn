const courseID = document.getElementById('CourseID').value
const searchBar = document.getElementById('lessonSearchbar')
const resultRow = document.getElementById('resultRow')


var refreshedRow = resultRow.cloneNode(false)
var generateIcons = function (id) {
    return `<hr/>
            <button class='cn-icon'><a href="${window.location.origin}/Lessons/Edit/${id}">Edit</a></button>
            <button class='cn-icon'><a href="${window.location.origin}/Lessons/Details/${id}">Details</button>
            <button name='danger' class='cn-icon'><a href="${window.location.origin}/Lessons/Delete/${id}">Delete</button>`
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
            cardText.innerHTML = lesson.Title + generateIcons(lesson.ID)

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