/*<input type="text" id="search-input" />
<input type="button" id="search-button" />
<div id="results"> </div> */
var results = document.getElementById("results")
var textInput = document.getElementById("search-input")
var button = document.getElementById("search-button")
var data = document.getElementById("data")
var points = document.getElementById("points")
var applyButton = document.getElementById("apply")
var displayQuestions;
//await axios.get(`/Review/GetQuestionForReview/?questionID=${review[index].QuestionID}`);
var questions = null;
async function getQuestions() {
     questions = await axios.get(`/Questions/GetAll/?ID=${data.getAttribute("val")}`)
     questions = questions.data
}
function applySearch(searchVal) {
    console.log(searchVal)
    displayQuestions = questions.filter(q => q.Text.includes(searchVal))
    console.log(displayQuestions)
    results.innerHTML = ''
    var resultsString = ""
    for (var question of displayQuestions) {
        results.innerHTML +=
            `<div><input type='checkbox' id='question-${question.ID}'>
             ${question.Text}</div>`
            } 
    }
getQuestions();
button.addEventListener("click", () => {
        console.log(`Textarea: ${textInput.value}`)
     applySearch(textInput.value)
})
applyButton.addEventListener("click", async () => {
    var selectedElements = []
    for (var question of displayQuestions) {
        var element = document.getElementById(`question-${question.ID}`)
        if (element.checked) {
            await axios.get(`/Questions/SetPoints/?ID=${question.ID}&Points=${points.value}`)
        }
    }
    console.log(selectedElements)
})