let index = 0;
let reviewArea = document.querySelector("#review-area");
let submitButton = document.querySelector("#question-submit");
let countArea = document.querySelector('#current');
let questionText = document.querySelector('#question-text');
let answer = document.querySelector("#question-answer")
let question = null;
let review = null;
let updateCounter = function () {
    countArea.innerHTML = `${(index + 1)}/${review.length}`
}
let loadNextFunction = async function () {
        submitButton.innerHTML = "Check";
        submitButton.removeEventListener('click', loadNextFunction, false);
        submitButton.addEventListener('click', submitAnswerFunction);
        reviewArea.classList.remove('review-card-success');
        reviewArea.classList.remove('review-card-fail');
        index++;
        updateCounter();
        question = await axios.get(`/Review/GetQuestionForReview/?questionID=${review[index].QuestionID}`);
        question = question.data;
        questionText.innerHTML = question.Text;

}
let submitAnswerFunction = async function (event) {
    event.preventDefault();
    const val = answer.value
    const response = await axios.get(`/Review/CheckQuestion/?questionID=${question.ID}&answer=${val}`);
    if (response.status == 200) {
        if (response.data.IsCorrect) {
            reviewArea.classList.add("review-card-success")
        }
        else {
            reviewArea.classList.add("review-card-fail")
        }
    }
    submitButton.innerHTML = "Continue"
    submitButton.removeEventListener('click', submitAnswerFunction, false);
    if (index < review.length - 1) {
        submitButton.addEventListener('click', loadNextFunction);
    }
    else {
        questionText.innerHTML = "You're all caught up. Check back later for more reviews!";
        submitButton.innerHTML = "Return Home";
        submitButton.addEventListener('click', function () {
            window.location.replace("/Home/Index")
        })
        answer.parentNode.removeChild(answer);
    }
    
}
var buildPage = async function () {
    questionText.innerHTML = "Loading...";
    review = await axios.get(`/Review/GetAllReviews`);
    review = review.data;
    if (review.length == 0) {
        questionText.innerHTML = "You're all caught up. Check back later for more reviews!";
        answer.parentNode.removeChild(answer);
        submitButton.innerHTML = "Return Home";
        submitButton.addEventListener('click', function () {
            window.location.replace("/Home/Index")
        })
    }
    question = await axios.get(`/Review/GetQuestionForReview/?questionID=${review[index].QuestionID}`);
    question = question.data;

    updateCounter();
    questionText.innerHTML = question.Text;
    submitButton.addEventListener('click', submitAnswerFunction)

}
buildPage();