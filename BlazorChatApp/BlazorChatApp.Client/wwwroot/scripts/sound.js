window.playsound = function (soundfilePath) {
    const audio = new Audio(soundfilePath);
    audio.play();
}