function autoSubmitForm() {
    var form = document.getElementById('actForm');
    var input = document.createElement('input');
    input.type = 'hidden';
    input.name = 'changed';
    input.value = 'dealerTurn';
    form.appendChild(input);

    form.submit();
}