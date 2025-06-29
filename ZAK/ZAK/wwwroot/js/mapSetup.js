
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
    markerGroup = L.markerClusterGroup();
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
    let markerData = JSON.parse(markerString);

    let classToUse = "";

    if (markerData.stretchingStatus == "Stretched") {
        if (markerData.hot == true) {
            classToUse = "div-icon-imp-str";
        }
        else {
            classToUse = "div-icon-reg-str";
        }
    } else {
        if (markerData.hot == true) {
            classToUse = "div-icon-imp-nstr";
        }
        else {
            classToUse = "div-icon-reg-nstr";
        }

    }

    const marker = L.marker(
        [markerData.lat, markerData.lon],
        {
            icon: L.divIcon({
                className: classToUse
            }
            )
        }

    );

    let selectOptions = "";
    for (let i = 0; i < markerData.brigadeCount; i++) {

        selectOptions +=
            `<option id="brigadeSelector" value="${i}.${markerData.id}">
                Бригада ${i + 1}
            </option>`;
    }


    const popup = L.popup()
        .setContent(
            `
                <b> ${markerData.id}</b>
                <hr>
                ${markerData.streetName + " " + markerData.houseNumber}
                <hr>
                ${markerData.operatorComment}
                <br>
                ${markerData.masterComment}
                <hr>
                <b>Додати до бригади:</b>
                <select class="custom-select"> 
                    ${selectOptions}
                </select>

            `
        );


    popup.maxHeight = 50;

    // marker.on('popupopen', () => {
    //     // Находим элемент <select> внутри popup
    //     const select = document.querySelector('.custom-select');
    //     if (select) {
    //         // Удаляем старые обработчики, чтобы избежать дублирования
    //         select.removeEventListener('change', handleSelectChange);
    //         // Добавляем новый обработчик
    //         select.addEventListener('change', (event) => handleSelectChange(event, marker));
    //     }
    // });

    marker.bindPopup(popup);
    markerGroup.addLayer(marker);
    map.addLayer(markerGroup);

}

// function handleSelectChange(value) {
//     {
//         const selectedOption = value.target.value;
//         const [brigadeNumber, applicationId] = selectedOption.split('.');
//         alert(`Бригада ${brigadeNumber} обрана для заявки ${applicationId}`);
//         leafleatMapController.invokeMethodAsync("ScheduleApplicationToBrigade", applicationId, brigadeNumber)
//     }
// }
function clearMap() {
    polylineGroup.clearLayers();
}
