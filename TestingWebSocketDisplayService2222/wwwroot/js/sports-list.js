var connection = new signalR.HubConnectionBuilder()
    .withUrl('/MyHub')
    .build();
//connection SignalR started
connection.start().then(function () {
     console.log('SignalR connected');
});
var receivedData = [];

var footballBtn = document.getElementById('btnFootball');
var baseballBtn = document.getElementById('btnBaseball');
var tennisBtn = document.getElementById('btnTennis');
var basketballBtn = document.getElementById('btnBasket');
var liveBtn = document.getElementById('btnLive');
var preMatchBtn = document.getElementById('btn-pre-match');
var selectList = document.getElementById('listSelect');
var selectListSports = document.getElementById('listSelectSports');
var currentSport = "";
var isLiveClicked = false;
var isPreMatchClicked = false;
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
// Selecting League
selectList.addEventListener("change", function () {
    var selectedValue = selectList.value;
    const buttons = document.querySelectorAll(".event-button");
    displayEventData("getting_list_items");
});

// Selecting Other Sports
selectListSports.addEventListener("change", function () {
    var selectedValue = selectListSports.value;
    currentSport = selectedValue;
    console.log('selected value = ' + currentSport);
    const buttons = document.querySelectorAll(".event-button");
    buttons.forEach(button => {
        button.classList.remove('btn-danger');
        button.classList.add('btn-primary');
    });
    divLivePreMatch.classList.remove('d-none');
    divLivePreMatch.classList.add('d-flex');
    restartButtonLive();
    restartButtonPrematch();
    if (selectListSports.value === "More Sports") {
        divLivePreMatch.classList.add('d-flex');

        divLivePreMatch.classList.add('d-none');
        displayEventData("displayAll");

    }
    else {
        displayEventData("getting_more_sports");

    }
});
// Displaying sport
function displayEventData(sport) {
    connection.off('UpdateData');
    rows = {};

    // Set the currentSport variable to the new sport
    //   currentSport = sport;
    var table = document.getElementById("eventsTable");
    var rowCount = table.rows.length;
    for (var i = rowCount - 1; i > 0; i--) {
        table.deleteRow(i);
    }
    // Define a dictionary to store the rows for each event ID
    var rows = {};
    //selectList.options.length = 0;
    var allItems = [];
    connection.on('UpdateData', function (data) {
        if (sport !== "getting_list_items") {
            for (var i = selectList.options.length - 1; i > 0; i--) {
                selectList.remove(i);
            }
        }
        allItems = [];

        Object.keys(data).forEach(function (eventId) {
            if (Array.isArray(data)) {
                receivedData = data; // Ažuriranje receivedData samo ako je data niz
                // Ostatak vaše logike za ažuriranje tabele ili druge manipulacije podacima
            }
            //console.log("EventID = " + eventId);
            var eventData = data[eventId];
            if (sport == "displayAll" || sport == "") {
                SetOptionsForLeagueList(allItems, eventData, selectList);
                SetRow(rows, table, eventId, eventData);
            }
            //else if (eventData.ir_status === "in_running" && sport == "in_running") {
            //    SetRow(rows, table, eventId, eventData);
            //}
            else if (eventData.sport === sport && !isLiveClicked && !isPreMatchClicked) {
                SetOptionsForLeagueList(allItems, eventData, selectList);
                SetRow(rows, table, eventId, eventData);
            }
            else if (eventData.sport === sport && isLiveClicked && eventData.ir_status == "in_running") {
                console.log("sport: " + sport);
                SetOptionsForLeagueList(allItems, eventData, selectList);
                SetRow(rows, table, eventId, eventData);
            }
            else if (eventData.sport === sport && isPreMatchClicked && eventData.ir_status == "pre_event") {
                console.log("sport: " + sport);
                SetOptionsForLeagueList(allItems, eventData, selectList);
                SetRow(rows, table, eventId, eventData);
            }
            else if (sport === "getting_list_items" && eventData.competition_name === selectList.value && !isLiveClicked && !isPreMatchClicked) {
                ///method for leagues
                SetRow(rows, table, eventId, eventData);
            }
            else if (sport === "getting_list_items" && eventData.competition_name === selectList.value && isLiveClicked && eventData.ir_status === "in_running") {
                ///method for leagues
                SetRow(rows, table, eventId, eventData);
            }
            else if (sport === "getting_list_items" && eventData.competition_name === selectList.value && isPreMatchClicked && eventData.ir_status === "pre_event") {
                ///method for leagues
                SetRow(rows, table, eventId, eventData);
            }
            else if (sport === "getting_more_sports" && eventData.sport == selectListSports.value && !isLiveClicked && !isPreMatchClicked) {
                SetOptionsForLeagueList(allItems, eventData, selectList);
                SetRow(rows, table, eventId, eventData);
            }
            else if (sport === "getting_more_sports" && eventData.sport == selectListSports.value && isLiveClicked && eventData.ir_status === "in_running") {
                SetOptionsForLeagueList(allItems, eventData, selectList);
                SetRow(rows, table, eventId, eventData);
            }
            else if (sport === "getting_more_sports" && eventData.sport == selectListSports.value && isPreMatchClicked && eventData.ir_status === "pre_event") {
                SetOptionsForLeagueList(allItems, eventData, selectList);
                SetRow(rows, table, eventId, eventData);
            }
        });
    });
}
function SetOptionsForLeagueList(allItems, eventData, selectList) {
    if (allItems.indexOf(eventData.competition_name) == -1) {

        allItems.push(eventData.competition_name);
        var option = document.createElement("option");
        option.text = eventData.competition_name;
        option.value = eventData.competition_name;
        selectList.add(option);
    }
}
function highlightButton(clickedButton, currentSport) {
    const buttons = document.querySelectorAll(".event-button");
    selectListSports.selectedIndex = 0;
    buttons.forEach(button => {
        if (button === clickedButton) {
            if (button.classList.contains('btn-danger')) {
                divLivePreMatch.classList.remove('d-flex');
                divLivePreMatch.classList.add('d-none');
                // nije kliknuto
                button.classList.remove('btn-danger');
                button.classList.add('btn-primary');
                console.log("current sport: EMPTY " );

                currentSport = "";
            }
            else {
                button.classList.remove('btn-primary');
                button.classList.add('btn-danger');
                divLivePreMatch.classList.remove('d-none');
                divLivePreMatch.classList.add('d-flex');
                console.log("current sport: " + currentSport);
            }
        }
        else {
            button.classList.remove('btn-danger');
            button.classList.add('btn-primary');
            console.log("current sport: " + currentSport);

        }
    });
}
// Getting All data when page is loaded
document.addEventListener("DOMContentLoaded", function () {
    HandlingSports();
    console.log("display All should be called ")
    displayEventData("displayAll");
});

function getImagePath(sport) {
    switch (sport) {
        case "ru":
            return "Icons/rugby-union.png";
        case "rl":
            return "Icons/rugby-league.png";
        case "baseball":
            return "Icons/baseball.png";
        case "af":
            return "Icons/american-football.png";
        case "basket":
            return "Icons/basketball.png";
        case "basket_ht":
            return "Icons/basketball_ht.jpg";
        case "boxing":
            return "Icons/boxing-gloves.png";
        case "cricket":
            return "Icons/cricket.png";
        case "fb":
        case "fb_ht":
        case "fb_et":
        case "fb_corn":
        case "fb_corn_ht":
        case "fb_htft":
            return "Icons/football-game.png";
        case "ih":
            return "Icons/hockey.png";
        case "tennis":
            return "Icons/tennis.png";
        default:
            return "";
    }
}

// Creating new Row for event
function createEventRow(eventData) {
    var imagePath = getImagePath(eventData.sport);
    return `
                  <td><img style="width:30px" src="${imagePath}" /></td>
                  <td>${eventData.competition_name}</td>
                  <td>${eventData.competition_country}</td>
                  <td>${eventData.home}</td>
                  <td>${eventData.away}</td>
                  <td>${eventData.ir_status}</td>
                  <td>${eventData.start_time}</td>
              `;
}

function SetRow(rows, table, eventId, eventData) {
    var row = rows[eventId];
    checkingRow(table, row, eventId, rows);
    row.innerHTML = createEventRow(eventData);
}
function checkingRow(table, row, eventId, rows) {
    if (!row) {
        // If no row exists, create a new one and add it to the table
        row = table.insertRow();
        rows[eventId] = row;
    }
}

function HandlingSports() {
    //Adding all sports to select list
    var sports = ["fb_ht", "fb_et", "fb_corn", "fb_corn_ht", "fb_htft", "af", "ru", "rl", "ih", "boxing"];
    sports.forEach(function (sport) {
        var option = document.createElement("option");
        option.value = sport;
        if (sport === "fb_ht") {
            option.text = "Football, first half";
            option.value = "fb_ht";
        }
        if (sport === "fb_et") {
            option.text = "Football, extra time";
            option.value = "fb_et";
        }
        if (sport === "fb_corn") {
            option.text = "Football, the number of corners awarded in 90 minutes";
            option.value = "fb_corn";
        }
        if (sport === "fb_corn_ht") {
            option.text = "Football, the number of corners, first half only";
            option.value = "fb_corn_ht";
        }
        if (sport === "fb_htft") {
            option.text = "Football, bets on combined result at half-time and full-time";
            option.value = "fb_htft";
        }
        if (sport === "af") {
            option.text = "American football (includes Canadian football)";
            option.value = "af";
        }
        if (sport === "ru") {
            option.text = "Rugby Union";
            option.value = "ru";
        }
        if (sport === "rl") {
            option.text = "Rugby League";
            option.value = "rl";
        }
        if (sport === "ih") {
            option.text = "Ice Hockey";
            option.value = "ih";
        }
        if (sport === "boxing") {
            option.text = "Boxing";
            option.value = "boxing";
        }
        document.getElementById("listSelectSports").appendChild(option);
    });
}