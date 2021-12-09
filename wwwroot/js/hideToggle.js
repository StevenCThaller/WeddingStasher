function hideToggle() {
    var hidden = document.getElementsByClassName('hide-toggle');

    for(var el of hidden) {
        if(el.classList.contains('hidden')) {
            el.classList.remove('hidden');
        } else {
            el.classList.add('hidden');
        }
    }
}