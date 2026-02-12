let dotNetRef = null;
let isDragging = false;
let sliderWrapper = null;

export function initializePortionSlider(dotNetReference) {
    dotNetRef = dotNetReference;
    sliderWrapper = document.querySelector('.portion-slider-wrapper');
}

export function startDrag(eventType) {
    isDragging = true;
    
    if (eventType === 'mouse') {
        document.addEventListener('mousemove', handleDrag);
        document.addEventListener('mouseup', stopDrag);
    } else if (eventType === 'touch') {
        document.addEventListener('touchmove', handleDrag);
        document.addEventListener('touchend', stopDrag);
    }
}

function handleDrag(e) {
    if (!isDragging || !sliderWrapper || !dotNetRef) return;
    
    e.preventDefault();
    
    const rect = sliderWrapper.getBoundingClientRect();
    const clientX = e.type.includes('touch') ? e.touches[0].clientX : e.clientX;
    const x = clientX - rect.left;
    const percentage = Math.max(0, Math.min(100, (x / rect.width) * 100));
    
    // Convert percentage to value (1-16)
    const minValue = 1;
    const maxValue = 16;
    const value = Math.round((percentage / 100) * (maxValue - minValue) + minValue);
    
    dotNetRef.invokeMethodAsync('UpdateValue', value);
}

function stopDrag() {
    isDragging = false;
    document.removeEventListener('mousemove', handleDrag);
    document.removeEventListener('mouseup', stopDrag);
    document.removeEventListener('touchmove', handleDrag);
    document.removeEventListener('touchend', stopDrag);
}

export function cleanup() {
    stopDrag();
    dotNetRef = null;
    sliderWrapper = null;
}
