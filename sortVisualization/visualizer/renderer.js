export function drawArray(array, highlights = []) {
  const container = document.getElementById('array-container');
  if (!container) {
    console.error('Error: #array-container not found in DOM');
    return;
  }
  container.innerHTML = '';
  const max = Math.max(...array, 1);
  const barWidth = container.offsetWidth / array.length;

  array.forEach((value, i) => {
    const bar = document.createElement('div');
    bar.classList.add('bar');
    bar.style.height = `${(value / max) * 100}%`;
    bar.style.width = `${barWidth}px`;
    if (highlights.includes(i)) {
      bar.classList.add('highlight');
    }
    container.appendChild(bar);
  });
  console.log('Array drawn:', array);
}

export function sleep(ms) {
  return new Promise(resolve => setTimeout(resolve, ms));
}

export function shuffleArray(array) {
  for (let i = array.length - 1; i > 0; i--) {
    const j = Math.floor(Math.random() * (i + 1));
    [array[i], array[j]] = [array[j], array[i]];
  }
  return array;
}

export function generateArray(size) {
  const array = Array.from({ length: size }, () => Math.floor(Math.random() * 100) + 1);
  console.log('Generated array:', array);
  return array;
}