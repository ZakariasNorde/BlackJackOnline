function autoSubmitForm(value) {
    var form = document.getElementById('actForm');
    var input = document.createElement('input');
    input.type = 'hidden';
    input.name = 'changed';
    input.value = value;
    form.appendChild(input);

    form.submit();
}

document.addEventListener('DOMContentLoaded', function () {
    const adjustCardSizes = () => {
        const cards = document.querySelectorAll('.blackjack-card');
        const cardCount = cards.length;
        let cardWidth, cardHeight;

        // Adjust card size based on the number of cards
        if (cardCount <= 8) {
            cardWidth = '140px';
            cardHeight = '190px';
        } else if (cardCount <= 14) {
            cardWidth = '112px';
            cardHeight = '152px';
        } else if (cardCount <= 20) {
            cardWidth = '93px';
            cardHeight = '126px';
        } else {
            cardWidth = '80px';
            cardHeight = '108px';
        }


        // Apply the new size to each card
        cards.forEach(card => {
            card.style.width = cardWidth;
            card.style.height = cardHeight;
        });
    };

    adjustCardSizes();

    // If cards are dynamically added or removed, you can call adjustCardSizes() again as needed
});