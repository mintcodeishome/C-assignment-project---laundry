// Add this function to show a confirmation dialog before submitting
form.addEventListener('submit', function(e) {
    // Validate form
    const name = document.getElementById('name').value;
    const phone = document.getElementById('phone').value;
    
    if (!name || !phone) {
        e.preventDefault();
        alert('Please fill in all required fields');
        return;
    }
    
    // Check if at least one item is selected
    let hasItems = false;
    for (const item in prices) {
        const quantity = parseInt(document.querySelector(`input[name="${item}"]`).value);
        if (quantity > 0) {
            hasItems = true;
            break;
        }
    }
    
    if (!hasItems) {
        e.preventDefault();
        alert('Please select at least one laundry item');
        return;
    }
    
    // Show confirmation dialog
    if (!confirm('Are you sure you want to submit your order?')) {
        e.preventDefault();
        return;
    }
    
    // Show notification
    notification.classList.add('show');
    
    // Hide notification after 3 seconds
    setTimeout(function() {
        notification.classList.remove('show');
    }, 3000);
});