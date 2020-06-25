var resultsPerPageInput = document.getElementById('results-per-page');
var resultsPerPageError = document.getElementById('results-per-page-error');
var blogListSection = document.getElementById('blog-list-section');
var paginationSection = document.getElementById('pagination')
var blogPosts = [];
var currentPage = 1;
function changePage(pageNumber) {
    currentPage = pageNumber;
    blogListSection.innerHTML = generateBlogResults();
    window.scrollTo(0, document.body.scrollHeight);

}
function generatePageButton(pageNumber) {
    //TODO add pagination on click
    return `<button onClick="changePage(${pageNumber})"> ${pageNumber}</button>`
}
function generatePagination()
{
    var resultsPerPage = resultsPerPageInput.value;
    var totalPages = (blogPosts.length / resultsPerPage);
    var rawhtml = '';
    if ( totalPages < 6)
    {
        for (i = 1; i <= (blogPosts.length / resultsPerPage); i++) {
            rawhtml += generatePageButton(i);
        }
    }
    return rawhtml;
}
function generateBlogResults() {
    var rawhtml = ''
    for (var i = (currentPage - 1) * resultsPerPageInput.value;
        i < (resultsPerPageInput.value * currentPage) && i < blogPosts.length; i++) {
        var blogElement =
        `<div class="card">
        <div class="card-title" style="text-align:center;">
            <br>
            <h5>${blogPosts[i].Title} </h5>
             ${blogPosts[i].Author} </br>
             ${blogPosts[i].FormattedDate} <hr/>
        </div>
        <div class="card-body">
                ${blogPosts[i].Text}
        </div>
    </div><br/>`
        rawhtml += blogElement;
    }
    return rawhtml;
}
async function handleResultsUpdate(event)
{
    var isNumeric = /^\d+$/.test(resultsPerPageInput.value);
    if (isNumeric) {
        resultsPerPageError.innerHTML = '';
        console.log(blogPosts)
        currentPage = 1;
        blogListSection.innerHTML = generateBlogResults();
        paginationSection.innerHTML = generatePagination();
    }
    else {
        resultsPerPageError.innerHTML = 'Error, please enter only numbers'
    }
}
async function run()
{
    resultsPerPageInput.addEventListener('keyup', handleResultsUpdate);
    var results = await axios.get('/BlogPosts/GetBlogPosts')
    blogPosts = results.data
    blogListSection.innerHTML = generateBlogResults();
}

run();
