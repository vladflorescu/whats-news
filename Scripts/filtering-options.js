$(document).ready(function () {
  var searchInput = $('.search-group .search-input');
  var searchButton = $('.search-group .search-button');

  searchInput.on('keydown', function (e) {
    if (e.keyCode === 13) {
      e.preventDefault();
      searchButton.click();
    }
  })

  $('.sort-group select').on('change', function (e) {
    searchInput.val("");
    searchButton.click();
  })
});