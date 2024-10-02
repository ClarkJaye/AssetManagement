/*$(document).ready(function () {*/
document.addEventListener('DOMContentLoaded', function () {
    $('.dropdown-submenu a.test').on("click", function (e) {
        $(this).next('ul').toggle();
        e.stopPropagation();
        e.preventDefault();
    });


    $('#deviceModal').on('hidden.bs.modal', function () {
        // Reset the modal content on close
        $(this).find('.modal-content').html('');
    });

    $('#deviceModal').on('shown.bs.modal', function () {
        // Enable the click event for the options in the modal
        $(this).find('.device-options a').click(function () {
            // Close the modal
            $('#deviceModal').modal('hide');

            // Handle the clicked option
            var href = $(this).attr('');
            // Do something with the href, such as redirecting to the selected page
            window.location.href = href;

            return false;
        });
    });

    var uppercaseInputs = document.querySelectorAll('input');
    uppercaseInputs.forEach(function (input) {
        input.addEventListener('input', function () {
            this.value = this.value.toUpperCase();
        });
    });

});


function mobileMenuOpen() {
    document.getElementById("gmDropdown").classList.toggle("show");
}

function toggleDropdown3() {
    var dropdown = document.getElementById("myStore");
    dropdown.classList.toggle("show");
}

function toggleDropdown4() {
    var dropdown = document.getElementById("myDept");
    dropdown.classList.toggle("show");
}
