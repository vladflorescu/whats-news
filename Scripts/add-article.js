$(document).ready(function () {
  var multiselect = $('div.category-select-wrapper select').first().multiselect();
  var multiselectDomOptions = multiselect.get()[0].children;
  var categoriesFromServer = mapToText(multiselectDomOptions);

  var addCategoryInput = $('.add-category-input');
  var hiddenInput = $('.hidden-categories-input');

  //$('div.category-select-wrapper select').on('change', function (evt) {
  //  if ($(evt.target).val() === 'other') {
  //    $('input.hidden-category-input').removeClass('u-is-hidden');
  //  } else {
  //    $('input.hidden-category-input').addClass('u-is-hidden');
  //    $('input.hidden-category-input').val('');
  //  }
  //});

  addCategoryInput.on('keydown', function (evt) {
    var optionName = addCategoryInput.val();

    if (evt.keyCode === 13 && optionName !== '') {
      var multiselectDomOptions = multiselect.get()[0].children;

      var keyOfOptionWithTheSameName = Object.keys(multiselectDomOptions)
        .filter(function (key) {
          return $(multiselectDomOptions[key]).text() === optionName
        })[0];

      var optionWithTheSameName =
        (jQuery.inArray(keyOfOptionWithTheSameName, [undefined, null]) === -1)
            ? multiselectDomOptions[keyOfOptionWithTheSameName]
            : null;

      if (optionWithTheSameName === null) {
        var greatestId = Math.max.apply(this, mapToValue(multiselectDomOptions));
        multiselect.append($('<option>', {
          value: greatestId + 1,
          text: addCategoryInput.val(),
          selected: true
        }));
      } else {
        $(optionWithTheSameName).attr('selected', 'selected');
      }

      multiselect.multiselect('rebuild');
      addToHiddenInput(addCategoryInput.val());
      addCategoryInput.val('');
    } else if (evt.keyCode === 220) {
      evt.preventDefault();
    }
  });

  multiselect.on('change', function () {
    multiselectDomOptions = multiselect.get()[0].children;

    var selectedCategoriesNames = Object.keys(multiselectDomOptions)
      .filter(function (key) {
        return $(multiselectDomOptions[key]).is(':selected');
      })
      .map(function (key) {
        return $(multiselectDomOptions[key]).text();
      })
    ;

    var hiddenInputCategories = hiddenInput.val()
      .split('|').filter(function (str) { return str !== ''; });

    //Remove unchecked categories from hidden input
    hiddenInputCategories.filter(function (str) {
      return selectedCategoriesNames.indexOf(str) < 0;
    }).forEach(function (str) {
      removeFromHiddenInput(str);
    });

    //Add rechecked categories to hidden input
    selectedCategoriesNames.filter(function (str) {
      return categoriesFromServer.indexOf(str) < 0
          && hiddenInputCategories.indexOf(str) < 0;;
    }).forEach(function (str) {
      addToHiddenInput(str);
    });
  })

  var articleTextarea = $('.article-text-area');

  articleTextarea.on('keydown.ignoreTabs', function (evt) {
    if (evt.keyCode === 9) { // tab was pressed
      // get caret position/selection
      var start = this.selectionStart;
      var end = this.selectionEnd;

      var $this = $(this);
      var value = $this.val();

      // set textarea value to: text before caret + tab + text after caret
      $this.val(value.substring(0, start)
                  + "\t"
                  + value.substring(end));

      // put caret at right position again (add one for the tab)
      this.selectionStart = this.selectionEnd = start + 1;

      // prevent the focus lose
      evt.preventDefault();
    }
  });

  var resizeTextarea = function() {
    articleTextarea.get(0).style.height = 'auto';
    articleTextarea.get(0).style.height = articleTextarea.get(0).scrollHeight + 'px';
  }

  $('.article-text-area').on('keydown.resizeTextarea', resizeTextarea);
  $('.article-text-area').on('change', resizeTextarea);

  $(".article-text-area").bind({
    paste: resizeTextarea,
    cut: resizeTextarea,
    drop: resizeTextarea
  });

  $(window).on('resize.resizeTextarea', resizeTextarea);

  function addToHiddenInput(value) {
    hiddenInput.val(hiddenInput.val() + value + '|');
  };

  function removeFromHiddenInput(value) {
    hiddenInput.val(hiddenInput.val().replace(value + '|', ''));
  };

  function mapToText(container) {
    return Object.keys(container).map(function (key) {
      return $(container[key]).text();
    });
  }

  function mapToValue(container) {
    return Object.keys(container).map(function (key) {
      return $(container[key]).val();
    });
  }
});