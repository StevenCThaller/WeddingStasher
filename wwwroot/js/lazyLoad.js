document.addEventListener("DOMContentLoaded", function() {
    var main = document.querySelector("main");
    var lazyLoadImages = document.querySelectorAll("img.lazy");
    
    var lazyLoadThrottleTimeout;

    function lazyLoad() {
        if(lazyLoadThrottleTimeout) {
            clearTimeout(lazyLoadThrottleTimeout);
        }

        lazyLoadThrottleTimeout = setTimeout(function() {
            var scrollTop = main.scrollTop;

            lazyLoadImages.forEach(function(img){
                if(img.offsetTop < (main.offsetHeight + scrollTop)) {
                    img.src = img.dataset.src;
                    img.classList.remove('lazy');
                }
            });

            if(lazyLoadImages.length == 0) {
                main.removeEventListener("scroll", lazyLoad);
                window.removeEventListener("resize", lazyLoad);
                window.removeEventListener("orientationChange", lazyLoad);
            }
        }, 20);

    }
    main.addEventListener("scroll", lazyLoad);
    window.addEventListener("resize", lazyLoad);
    window.addEventListener("orientationChange", lazyLoad);
})