var searchBarElem = document.getElementById('searchbar');
var searchInputElem = document.getElementById('search-input');
var studentListElem = document.getElementById('students-list');
var dataElement = document.getElementById('data');

var studentNameElem = document.getElementById('student-name');
var studentStartDateElem = document.getElementById('student-start-date')
var studentAverageReviewsElem = document.getElementById('student-average-reviews')
var studentAverageLessonsElem = document.getElementById('student-average-lessons')
var studentProjectedCompletionElem = document.getElementById('student-projected-completion')
var studentEmailElem = document.getElementById('student-email')

//TODO: add add statistics generating functionality
async function handleStudentSelect(id)
{
    //TODO: handle variable courseID's
    var response = await axios.get(`/Students/GetStudentStatistics/?studentID=${id}&courseID=${dataElement.getAttribute('courseID')}`)
    if (response.status == 200) {
        var stats = response.data;
        studentNameElem.innerHTML = `${stats.FirstName} ${stats.LastName}` 
        studentStartDateElem.innerHTML = stats.FormattedStartDate
        studentAverageReviewsElem.innerHTML = stats.AverageReviewsPerDay.toFixed(2)
        studentAverageLessonsElem.innerHTML = stats.AverageNewLessonsPerDay.toFixed(2)
        studentProjectedCompletionElem.innerHTML = stats.FormattedProjectedCompletionDate
        studentEmailElem.innerHTML = stats.Email
        studentEmailElem.href =`mailto:${stats.Email}`
    }
}
function generateCard(id, name) {
    var studentCard = document.createElement('div');
    studentCard.innerHTML = 
    `<div class="card">
        <div class="card-title">
            ${name}
        </div>
        <div class="card-body">
            <button onClick="handleStudentSelect(${id})"> Generate Statistics </button>
        </div>
    </div>`
    studentListElem.appendChild(studentCard);
}
async function generateSearchResults(searchText) 
{
    var result = await axios.get(`/Cours/GetEnrolledStudents/${dataElement.getAttribute('courseID')}`);
    if (result.status == 200) {
        var studentList = result.data;
        if (searchText != null) {
            studentList = studentList.filter(function (student) {
                return (student.FirstName.toLowerCase().includes(searchText.toLowerCase())
                    || student.LastName.toLowerCase().includes(searchText.toLowerCase()));
            });
        }
        //deletes any previous elements
        studentListElem.innerHTML = '';
        for (var student of studentList) {
            generateCard(student.ID, `${student.FirstName} ${student.LastName}`)
        }
    }
    //handle issues here
    else {

    } 
}

async function run()
{
    generateSearchResults();
    searchInputElem.addEventListener('keyup', function (event) {
        generateSearchResults(event.srcElement.value)
        console.log()
    })
}
run();