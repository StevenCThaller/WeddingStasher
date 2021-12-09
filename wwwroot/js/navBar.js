var expanded = false;
var navMenu = document.getElementById('navbarSupportedContent');
var navItems = navMenu.children[0].children;
var i = 0;

window.addEventListener('click', function(e){
    if(!this.document.getElementById('wpNavBar').contains(e.target) && expanded == true) {
        expanded = false;
        shrinkMenu();
    }
})


function menuClick() {
    expanded ? shrinkMenu() : expandMenu();

    expanded = !expanded;
}

async function expandMenu() {
    var navMenu = document.getElementById('navbarSupportedContent');
    navMenu.classList.remove('collapse');
    navMenu.classList.add('grow-down');
    i = 0;
    var navItems = navMenu.children[0].children;

    await waitLoop(navItems, i, 'slide-in', 'slide-out');
    
    // .forEach(child => setTimeout(() => child.classList.add('slideIn'), 300));
}

function waitLoop(navItems, i, addClass, removeClass) {
    navItems[i].classList.add(addClass)
    navItems[i].classList.remove(removeClass)
    
    i++;
    if(i < navItems.length) {
        return waitLoop(navItems, i, addClass, removeClass);
    }
    
}

async function addSlideOut(navItem){
    navItem.classList.add('slide-out')
}

async function shrinkMenu() {
    var navMenu = document.getElementById('navbarSupportedContent');
    i = 0;
    var navItems = navMenu.children[0].children;
    await waitLoop(navItems, i, 'slide-out', 'slide-in');
    
    navMenu.classList.remove('grow-down');
    navMenu.classList.add('collapse');
    // .forEach(child => setTimeout(() => child.classList.add('slideIn'), 300));
}