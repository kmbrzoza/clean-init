function createRipple(event) {
    const button = event.currentTarget;
    const circle = document.createElement("span");
    const diameter = Math.max(button.clientWidth, button.clientHeight);
    const radius = diameter / 2;

    circle.style.width = circle.style.height = `${diameter}px`;
    circle.style.left = `${event.clientX - (button.getBoundingClientRect().left + radius)}px`;
    circle.style.top = `${event.clientY - (button.getBoundingClientRect().top + radius)}px`;
    circle.classList.add("ripple");

    const ripple = button.getElementsByClassName("ripple")[0];

    if (ripple) {
        ripple.remove();
    }

    button.appendChild(circle);

    setTimeout(() => {
        button.blur();
    }, 1);
}

function removeRipple(event) {
    const ripple = event.currentTarget.getElementsByClassName("ripple")[0];
    if (ripple) {
        ripple.remove();
    }
}

const buttons = document.getElementsByTagName("button");
for (const button of buttons) {
    button.addEventListener("mousedown", createRipple);
    button.addEventListener("animationend", removeRipple);
}
