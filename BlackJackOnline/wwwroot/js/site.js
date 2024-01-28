function autoSubmitForm(value) {
    var form = document.getElementById('actForm');
    var input = document.createElement('input');
    input.type = 'hidden';
    input.name = 'changed';
    input.value = value;
    form.appendChild(input);

    form.submit();
}