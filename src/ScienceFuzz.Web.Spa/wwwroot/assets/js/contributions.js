function loadContributionChart(id, input) {
    console.log(input);
    var ctx = document.getElementById(id);
    new Chart(ctx, input);
}