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
