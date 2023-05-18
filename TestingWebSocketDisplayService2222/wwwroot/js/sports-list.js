
//// Displaying sport
//function displayEventData(sport) {
//    connection.off('UpdateData');
//    rows = {};

//    // Set the currentSport variable to the new sport
//    //   currentSport = sport;
//    var table = document.getElementById("eventsTable");
//    var rowCount = table.rows.length;
//    for (var i = rowCount - 1; i > 0; i--) {
//        table.deleteRow(i);
//    }
//    // Define a dictionary to store the rows for each event ID
//    var rows = {};
//    //selectList.options.length = 0;
//    var allItems = [];
//    connection.on('UpdateData', function (data) {
//        if (sport !== "getting_list_items") {
//            for (var i = selectList.options.length - 1; i > 0; i--) {
//                selectList.remove(i);
//            }
//        }
//        allItems = [];

//        Object.keys(data).forEach(function (eventId) {
//            console.log("Loaded sport: " + sport);
//            //console.log("EventID = " + eventId);
//            var eventData = data[eventId];
//            if (sport == "displayAll" || sport == "") {
//                SetOptionsForLeagueList(allItems, eventData, selectList);
//                SetRow(rows, table, eventId, eventData);
//            }
//            //else if (eventData.ir_status === "in_running" && sport == "in_running") {
//            //    SetRow(rows, table, eventId, eventData);
//            //}
//            else if (eventData.sport === sport && !isLiveClicked && !isPreMatchClicked) {
//                SetOptionsForLeagueList(allItems, eventData, selectList);
//                SetRow(rows, table, eventId, eventData);
//            }
//            else if (eventData.sport === sport && isLiveClicked && eventData.ir_status == "in_running") {
//                console.log("sport: " + sport);
//                SetOptionsForLeagueList(allItems, eventData, selectList);
//                SetRow(rows, table, eventId, eventData);
//            }
//            else if (eventData.sport === sport && isPreMatchClicked && eventData.ir_status == "pre_event") {
//                console.log("sport: " + sport);
//                SetOptionsForLeagueList(allItems, eventData, selectList);
//                SetRow(rows, table, eventId, eventData);
//            }
//            else if (sport === "getting_list_items" && eventData.competition_name === selectList.value && !isLiveClicked && !isPreMatchClicked) {
//                ///method for leagues
//                SetRow(rows, table, eventId, eventData);
//            }
//            else if (sport === "getting_list_items" && eventData.competition_name === selectList.value && isLiveClicked && eventData.ir_status === "in_running") {
//                ///method for leagues
//                SetRow(rows, table, eventId, eventData);
//            }
//            else if (sport === "getting_list_items" && eventData.competition_name === selectList.value && isPreMatchClicked && eventData.ir_status === "pre_event") {
//                ///method for leagues
//                SetRow(rows, table, eventId, eventData);
//            }
//            else if (sport === "getting_more_sports" && eventData.sport == selectListSports.value && !isLiveClicked && !isPreMatchClicked) {
//                SetOptionsForLeagueList(allItems, eventData, selectList);
//                SetRow(rows, table, eventId, eventData);
//            }
//            else if (sport === "getting_more_sports" && eventData.sport == selectListSports.value && isLiveClicked && eventData.ir_status === "in_running") {
//                SetOptionsForLeagueList(allItems, eventData, selectList);
//                SetRow(rows, table, eventId, eventData);
//            }
//            else if (sport === "getting_more_sports" && eventData.sport == selectListSports.value && isPreMatchClicked && eventData.ir_status === "pre_event") {
//                SetOptionsForLeagueList(allItems, eventData, selectList);
//                SetRow(rows, table, eventId, eventData);
//            }
//        });
//    });
//}


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

