const ID = document.getElementById("CourseID").value
const checkbox = document.getElementById("ValidCheckbox")
checkbox.addEventListener("click", async function () {
    const response = await axios.get(`/Cours/SetCourseValidationState/?courseID=${ID}&valid=${checkbox.checked}`)
})