var footballBtn = document.getElementById('btnFootball');
var baseballBtn = document.getElementById('btnBaseball');
var tennisBtn = document.getElementById('btnTennis');
var basketballBtn = document.getElementById('btnBasket');
var liveBtn = document.getElementById('btnLive');
var preMatchBtn = document.getElementById('btn-pre-match');
var selectListSports = document.getElementById('listSelectSports');
var divLivePreMatch = document.getElementById('show-options');

footballBtn.addEventListener("click", function () {
    restartButtonLive();
    restartButtonPrematch();
    currentSport = "fb";
    highlightButton(footballBtn, currentSport);
    displayEventData(currentSport);

});
baseballBtn.addEventListener("click", function () {
    restartButtonLive();
    restartButtonPrematch();
    currentSport = "baseball";
    highlightButton(baseballBtn, currentSport);
    displayEventData(currentSport);
});
tennisBtn.addEventListener("click", function () {
    restartButtonLive();
    restartButtonPrematch();
    currentSport = "tennis";
    highlightButton(tennisBtn, currentSport);
    displayEventData(currentSport);
});
basketballBtn.addEventListener("click", function () {
    restartButtonLive();
    restartButtonPrematch();

    currentSport = "basket";
    highlightButton(basketballBtn, currentSport);
    displayEventData(currentSport);
});
preMatchBtn.addEventListener("click", function () {
    restartButtonLive();
    if (isPreMatchClicked) {
        highlightButtonPrematch();
        isPreMatchClicked = false;
        if (currentSport != "") {
            displayEventData(currentSport);
        }
        else {
            displayEventData("displayAll");
        }
    }
    else {
        isPreMatchClicked = true;
        //console.log('isPreMatchClicked: ' + isPreMatchClicked);
        highlightButtonPrematch();
        if (currentSport != "") {
            //console.log('calling display: currentSport' + currentSport);
            displayEventData(currentSport);
        }
        else {
            // console.log('calling display: in_running' + currentSport);

            displayEventData("pre_event");

        }
    }
})
liveBtn.addEventListener("click", function () {
    restartButtonPrematch();
    console.log('currentSport: ' + currentSport);
    if (isLiveClicked) {
        highlightButtonLive();
        isLiveClicked = false;
        if (currentSport != "") {
            displayEventData(currentSport);
        }
        else {
            displayEventData("displayAll");

        }
    }
    else {
        isLiveClicked = true;
        highlightButtonLive();

        if (currentSport != "") {
            displayEventData(currentSport);
        }
        else {
            displayEventData("in_running");
        }
    }
});

// Changing button background if button is clicked
function highlightButtonLive() {
    // selectListSports.selectedIndex = 0;
    if (liveBtn.classList.contains('btn-primary')) {
        liveBtn.classList.remove('btn-primary');
        liveBtn.classList.add('btn-danger');
    }
    else {
        liveBtn.classList.remove('btn-danger');
        liveBtn.classList.add('btn-primary');
    }
}
function highlightButtonPrematch() {
    // selectListSports.selectedIndex = 0;
    if (preMatchBtn.classList.contains('btn-primary')) {
        preMatchBtn.classList.remove('btn-primary');
        preMatchBtn.classList.add('btn-danger');
    }
    else {
        preMatchBtn.classList.remove('btn-danger');
        preMatchBtn.classList.add('btn-primary');
    }
}
function restartButtonLive() {
    //selectListSports.selectedIndex = 0;
    isLiveClicked = false;
    liveBtn.classList.remove('btn-danger');
    liveBtn.classList.add('btn-primary');

}
function restartButtonPrematch() {
    //selectListSports.selectedIndex = 0;
    isPreMatchClicked = false;
    preMatchBtn.classList.remove('btn-danger');
    preMatchBtn.classList.add('btn-primary');
}