$(document).ready(function () {
  var invisibleSubmitButton = $('.manage-users-invisible-submit-button');
  var toggles = $('.table > tbody .toggle');
  var hiddenUserName = $('.hidden-user-name');
  var hiddenRoleValue = $('.hidden-role-value');
  var hiddenToggleValue = $('.hidden-toggle-value');

  toggles.bootstrapSwitch({
    onText: "True",
    offText: "False",
    onSwitchChange: function (ev) {
      hiddenUserName.val($(ev.target).attr('data-user-name'));
      hiddenRoleValue.val("publisher");
      hiddenToggleValue.val($(ev.target).is(':checked'));
      invisibleSubmitButton.click();
    }
  });
});