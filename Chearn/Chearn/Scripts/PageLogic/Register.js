const studentInput = 'yearsOfEducation'
const hidablePairs = {
    student: 'yearsOfEducationForm',
    instructor: 'hasDoctorateForm' }
const userTypeDoc = document.querySelector('#userType')
hideAllExcept = function (userType)
{
    //resets studentInput
    document.querySelector(`#${studentInput}`).value = ''

    //Hides all form inputs in Register.cshtml except for selected userType
    for (val in hidablePairs)
    {
        if (val == userType) {
            document.querySelector(`#${hidablePairs[val]}`).setAttribute('style', 'visibility:visible')
        }
        else {
            document.querySelector(`#${hidablePairs[val]}`).setAttribute('style', 'visibility:hidden')
        }
    }
}
hideAllExcept(document.querySelector('#userType').value)
userTypeDoc.addEventListener('change', function (e) {
    hideAllExcept(e.target.value.toLowerCase())
})