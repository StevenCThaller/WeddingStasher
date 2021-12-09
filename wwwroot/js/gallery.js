const media = []
var index = 0;
var modalStatus = false;

function fetchPictures() {
    fetch('/media/images')
        .then(response => response.json())
        .then(images => startSlideShow(images.value.results))
        .catch(err => alert(err));
}

function updatePictures() {
    fetch('/media/images')
        .then(response => response.json())
        .then(images => {
            var imgs = images.value.results;
            for(var img of imgs) {
                if(media.filter(m => m.src == img.url).length == 0) {
                    var newImg = document.createElement('img');
                    newImg.classList.add('modal-carousel-item');
                    newImg.src = img.url;
                    newImg.alt = "Wedding Goings On"
                    media.push(newImg);
                }
            }
            console.log(imgs);
        })
        .catch(err => console.log(error))
}

function startSlideShow(images) {
    for(var img of images) {
        var newImg = document.createElement('img');
        newImg.classList.add('modal-carousel-item');
        newImg.src = img.url;
        newImg.alt = "Wedding Goings On"
        media.push(newImg);
    }

    insertMedia(i);

    setInterval(nextMedia,10000);
    setInterval(updatePictures, 100000);
}



const galleryItems = document.getElementsByClassName("gallery-item");

for(var item of galleryItems) {
    if(item.tagName == 'IMG') {
        media.push(item)
    } else {
        media.push(item.firstElementChild)
    }
}

function toggleModal(i) {
    var body = document.getElementsByTagName('body');
    body = body[0];
    var gallery = document.getElementById('gallery');

    // $('#gallery').modal('toggle')
    if(modalStatus == false) {
        body.classList.add('modal-open');
        gallery.classList.add('show');
        gallery.style = 'display: block;';
        gallery.ariaModal = true;
        gallery.ariaHidden = false;
        var backdrop = document.createElement('div');
        backdrop.classList.add('modal-backdrop');
        backdrop.classList.add('fade');
        backdrop.classList.add('show');
        body.appendChild(backdrop);
        insertMedia(i)
        gallery.addEventListener('click', clickOutOfMedia);
        index = i;
    } else {
        body.classList.remove('modal-open');
        gallery.classList.remove('show');
        gallery.style = '';
        gallery.ariaModal = false;
        gallery.ariaHidden = true;
        body.removeChild(body.lastChild);
        gallery.removeEventListener('click', clickOutOfMedia);
        index = i;
    }
    
    


    modalStatus = !modalStatus;
    
    // prevButton.insertBefore(media[item])    
}

function clickOutOfMedia(e) {
    // console.log(e.target);
    if(!e.target.classList.contains('carousel-click')) {
        // console.log("should close");
        var gallery = document.getElementById('gallery')
        gallery.removeEventListener('click', clickOutOfMedia);
        tearDown();
    }
}

function insertMedia(i) {
    var mediaElement = media[i];
    var mediaLocation = document.getElementById("modal-media-home");
    mediaLocation.innerHTML = "";
    var mediaType = mediaElement.tagName == 'VIDEO' ? 'video' : 'img';
    var newMedia = createMedia(mediaType, mediaElement);
    mediaLocation.appendChild(newMedia)
}

// function createImg(img) {
//     var newImg = document.createElement('img');
//     newImg.src = img.src;
//     newImg.alt = img.alt;
//     newImg.classList.add('modal-carousel-item');
//     new.classList.add('carousel-click');
//     return newImg;
// }

function createMedia(media, element) {
    var newMedia = document.createElement(media);
    newMedia.src = element.src;
    
    if(media == 'video') {
        newMedia.type = element.type;
        newMedia.controls = true;
        newMedia.autoplay = true;
        newMedia.loop = true;
    } else {
        newMedia.alt = element.alt;
    }
    newMedia.classList.add('modal-carousel-item');
    newMedia.classList.add('carousel-click');
    return newMedia;
}

function createVideo(video) {
    var newVideo = document.createElement('video');
    newVideo.src = video.src;
    newVideo.type = video.type;
    newVideo.classList.add('modal-carousel-item');
    newVideo.classList.add('carousel-click');
    newVideo.controls = true;
    newVideo.autoplay = true;
    return newVideo;
}

function selectMedia(el) {
    var idx;
    idx = el.tagName == 'DIV' ? i = media.indexOf(el.firstElementChild) : media.indexOf(el);
    toggleModal(idx);
}

function nextMedia() {
    if(index == media.length - 1) {
        index = 0;
    } else {
        index++;
    }
    insertMedia(index);
}

function prevMedia() {
    if(index == 0) {
        index = media.length - 1;
    } else {
        index--;
    }
    insertMedia(index);
}

function tearDown() {
    var toClear = document.getElementById("modal-media-home");
    toClear.innerHTML = "";
    toggleModal();
    index = 0;
    // window.removeEventListener('click');
}