
var map;

var polylineGroup;
var markerGroup;

function initMap() {
    map = L.map('map').setView([50.4503422102082, 30.524112747535554], 13);
    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    }).addTo(map);

    polylineGroup = L.layerGroup().addTo(map);
    markerGroup = L.layerGroup().addTo(map);
}

function drawPath(coordinates, color) {


    let cords = new Array();

    for (let i = 0; i < coordinates.split(' ').length - 1; i += 2) {
        cords.push([parseFloat(coordinates.split(' ')[i]), parseFloat(coordinates.split(' ')[i + 1])]);
    }

    const polyline = L.polyline(cords,
        {
            color: color,
            weight: 6,
            opacity: 0.7,
        });
    polylineGroup.addLayer(polyline);
}



function drawMarker(markerString) {

    let markerData = markerString.split(' ');

    let stretched = markerData[3];
    let important = markerData[4];

    let classToUse = "";

    if (stretched == "Stretched") {
        if (important == "True")
            classToUse = "div-icon-imp-str";
        else
            classToUse = "div-icon-reg-str";
    } else {
        if (important == "True")
            classToUse = "div-icon-imp-nstr";
        else
            classToUse = "div-icon-reg-nstr";

    }

    const marker = L.marker(
        [parseFloat(markerData[0]), parseFloat(markerData[1])],
        {
            icon: L.divIcon({
                className: classToUse
            }
            )
        }

    ).addTo(map);
    marker.bindPopup(markerData[2]);
    //markerGroup.addLayer(marker);
}

function clearMap() {
    polylineGroup.clearLayers();
}