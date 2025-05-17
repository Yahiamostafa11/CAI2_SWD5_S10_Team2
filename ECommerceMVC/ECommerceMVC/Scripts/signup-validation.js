document.getElementById('signupForm').addEventListener('submit', function (event) {
    event.preventDefault();

    // Clear previous error messages
    document.getElementById('nameError').textContent = '';
    document.getElementById('emailError').textContent = '';
    document.getElementById('ageError').textContent = '';
    document.getElementById('passwordError').textContent = '';
    document.getElementById('message').textContent = '';

    // Get form values
    const name = document.getElementById('name').value;
    const email = document.getElementById('email').value;
    const age = parseInt(document.getElementById('age').value);
    const password = document.getElementById('password').value;

    // Validation
    let valid = true;

    if (name.length <= 3) {
        document.getElementById('nameError').textContent = 'Name must be more than 3 characters.';
        valid = false;
    }

    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailPattern.test(email)) {
        document.getElementById('emailError').textContent = 'Email must be in a valid format.';
        valid = false;
    }

    if (age <= 18) {
        document.getElementById('ageError').textContent = 'Age must be greater than 18.';
        valid = false;
    }

    const passwordPattern = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
    if (!passwordPattern.test(password)) {
        document.getElementById('passwordError').textContent = 'Password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, one number, and one special character.';
        valid = false;
    }

    // If valid, save to local storage
    if (valid) {
        const userData = {
            name: name,
            email: email,
            age: age,
            password: password
        };
        localStorage.setItem('userData', JSON.stringify(userData));
        document.getElementById('message').textContent = 'Sign up successful! Redirecting to home page.';
        window.location.href = 'Home.html'; // Redirect to home page
    }
});

// Call checkLogin on page load
function checkLogin() {
    const storedData = JSON.parse(localStorage.getItem('userData'));
    if (storedData) {
        // Here you can implement login logic
        console.log('User data found:', storedData);
    } else {
        console.log('No user data found.');
    }
}

// Call checkLogin on page load
window.onload = checkLogin;
