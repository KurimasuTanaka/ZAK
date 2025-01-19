
var map;

var polylineGroup;

function initMap() {
    map = L.map('map').setView([50.4503422102082, 30.524112747535554], 13);
    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    }).addTo(map);

    polylineGroup = L.layerGroup().addTo(map);
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

function clearMap() {
    polylineGroup.clearLayers();
}