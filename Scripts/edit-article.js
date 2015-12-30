$(document).ready(function () {
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

  var resizeTextarea = function () {
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
});