function validatePage() {
  return (validateRemoteSource() && validateArticleTextarea());
}

function validateRemoteSource() {
  if ($('.remote-source-group .remote-cbox input').is(':checked')) {
    if ($('.remote-source-group .source-input').val().trim() === '') {
      $('.content-and-source-validator').text('The external source can\'t be empty.');
      $('.content-and-source-validator').removeClass('u-is-invisible');
      return false;
    } else {
      $('.content-and-source-validator').addClass('u-is-invisible');
    }
  }
  return true;
}

function validateArticleTextarea() {
  if (!$('.remote-source-group .remote-cbox input').is(':checked')) {
    if ($('.article-text-area').val().trim() === '') {
      $('.content-and-source-validator').text('The article can\'t be empty.');
      $('.content-and-source-validator').removeClass('u-is-invisible');
      return false;
    } else {
      $('.content-and-source-validator').addClass('u-is-invisible');
    }
  }
  return true;
}