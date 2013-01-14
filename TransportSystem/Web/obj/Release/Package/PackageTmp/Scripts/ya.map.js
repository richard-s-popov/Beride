var myMap;
ymaps.ready(function () {
    // Создание экземпляра карты и его привязка к созданному контейнеру
    myMap = new ymaps.Map("YMapsID", {
        center: [56.496618, 84.979832],
        zoom: 10
    });

    // Центр карты
    var center;

    // Масштаб
    var zoom = 7;

    // Получение информации о местоположении пользователя
    if (YMaps.location) {
        center = YMaps.GeoPoint(YMaps.location.longitude, YMaps.location.latitude);
        if (YMaps.location.zoom) {
            zoom = YMaps.location.zoom;
        }

        myMap.openBalloon(center, "Место вашего предположительного местоположения:<br/>"
            + (YMaps.location.country || "")
                + (YMaps.location.region ? ", " + YMaps.location.region : "")
                    + (YMaps.location.city ? ", " + YMaps.location.city : "")
        );
    } else {
        center = new YMaps.GeoPoint(56.496618, 84.979832);
    }

    // Установка для карты ее центра и масштаба
    myMap.setCenter([center.getX(), center.getY()], zoom);
    myMap.controls.add('typeSelector');
    myMap.controls.add('smallZoomControl');
});