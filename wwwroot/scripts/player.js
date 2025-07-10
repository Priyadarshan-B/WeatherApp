window.togglePlayPause = (id) => {
  const audio = document.getElementById(id);
  if (!audio) return;
  if (audio.paused) audio.play();
  else audio.pause();
};

window.seekAudio = (id, time) => {
  const audio = document.getElementById(id);
  if (audio) audio.currentTime = time;
};

window.startSliderSync = (id, dotnetObj) => {
  const audio = document.getElementById(id);
  if (!audio) return;
  setInterval(() => {
    if (audio && dotnetObj) {
      dotnetObj.invokeMethodAsync(
        "UpdateSlider",
        audio.currentTime,
        audio.duration || 0
      );
    }
  }, 500);
};

window.getElementWidth = (element) => {
  if (element && element.getBoundingClientRect) {
    return element.getBoundingClientRect().width;
  }
  return 300; // fallback width
};
