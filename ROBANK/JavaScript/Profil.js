//Declarare elemente html

const profilePhotoDiv = document.querySelector('.profilePhoto');
const photo = document.querySelector('#photo');
const image = document.querySelector('#profileImg');
const file = document.querySelector('#file');
const chooseBtn = document.querySelector('#chooseBtn');
const saveBtn = document.querySelector('#saveBtn');

const cldDepozitNou = document.querySelector('#cldDepozitNou');

function divDepozitNouClick() {
    cldDepozitNou.style.display = "none";
}

profilePhotoDiv.addEventListener('mouseenter', function () {
    chooseBtn.style.display = "block";
});


profilePhotoDiv.addEventListener('mouseleave', function () {
    chooseBtn.style.display = "none";
});

//Alegere poza

file.addEventListener('change', function () {

    var input = document.getElementById('file');

    var file = input.files[0];

    if ((file.size / 1024) / 1024 < 4) {

        image.style.display = "none";

        saveBtn.style.display = "block"

        const choosedFile = this.files[0];

        if (choosedFile) {
            const read = new FileReader();

            read.addEventListener('load', function () {
                photo.setAttribute('src', read.result);
            });

            read.readAsDataURL(choosedFile);
        }

    }

    else {

        alert("Dimensiunea imaginii este prea mare!");

    }

});

