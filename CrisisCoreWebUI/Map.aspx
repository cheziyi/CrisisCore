<%@ Page Title="Real-Time Monitoring" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Map.aspx.cs" Inherits="CrisisCoreWebUI.Map" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        #map {
            height: 680px;
        }
        #legend {
            padding-top: 20px;
            padding-bottom: 20px;
        }
        .box {
            float: left;
            width: 20px;
            height: 20px;
            margin-right: 5px;
            border: 1px solid rgba(0, 0, 0, .2);
        }
        .ne {
            background: #FF7DB8;
        }
        .nw {
            background: #A8DDAD;
        }
        .se {
            background: #D4EFBF;
        }
        .c {
            background: #EAC97F;
        }
        .sw {
            background: #DB89EF;
        }
    </style>
    <div class="row">
        <div class="col-lg-12">
            <div id="map"></div>
            <div id="legend">
                <img src="http://maps.google.com/mapfiles/ms/icons/orange.png"/> Dengue &nbsp &nbsp &nbsp &nbsp
                <img src="http://maps.google.com/mapfiles/ms/icons/pink.png"/> Zika &nbsp &nbsp &nbsp &nbsp
                <img src="http://maps.google.com/mapfiles/ms/icons/blue.png"/> Haze &nbsp &nbsp &nbsp &nbsp
                <img src="http://maps.google.com/mapfiles/ms/icons/green.png"/> Ambulance &nbsp &nbsp &nbsp &nbsp
                <img src="http://maps.google.com/mapfiles/ms/icons/purple.png"/> Rescue & Evac &nbsp &nbsp &nbsp &nbsp
                <img src="http://maps.google.com/mapfiles/ms/icons/red.png"/> Fire-fighting &nbsp &nbsp &nbsp &nbsp
                <img src="http://maps.google.com/mapfiles/ms/icons/yellow.png"/> Gas Leak
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-lg-12">
            <h3>Emergencies & Assistance</h3>
            <table class="table table-hover" id="table">
                <thead>
                    <tr>
                        <th>Area</th>
                        <th>Dengue Incidents</th>
                        <th>Zika Incidents</th>
                        <th>Haze PSI Level</th>
                        <th>Ambulance</th>
                        <th>Rescue & Evac</th>
                        <th>Fire-Fighting</th>
                        <th>Gas Leak</th>
                    </tr>
                </thead>
                <tbody>
                </tbody>
            </table>
        </div>
    </div>

    <script>
        var api = "http://api.crisiscore.cczy.io/";
        var map;
        var markers = [];
        var legend;
        var areas, weather, incidents;
        var neCoords = [], nwCoords = [], seCoords = [], cCoords = [], swCoords = [];
        var nePolygon, nwPolygon, sePolygon, cPolygon, swPolygon;
        var ne, nw, se, c, sw;
        
        $(document).ready(function () {
            // Getting polygons coordinates
            $.ajax({
                type: "GET",
                url: api + "Area/GetAllAreas",
                contentType: "application/json",
                async: false,
                success: function (data) {
                    areas = data;

                    // NORTH EAST
                    for (var key in areas[1].AreaBoundary) {
                        neCoords.push({ lat: areas[1].AreaBoundary[key].Latitude, lng: areas[1].AreaBoundary[key].Longitude });
                    }

                    // NORTH WEST
                    for (var key in areas[2].AreaBoundary) {
                        nwCoords.push({ lat: areas[2].AreaBoundary[key].Latitude, lng: areas[2].AreaBoundary[key].Longitude });
                    }

                    // SOUTH EAST
                    for (var key in areas[3].AreaBoundary) {
                        seCoords.push({ lat: areas[3].AreaBoundary[key].Latitude, lng: areas[3].AreaBoundary[key].Longitude });
                    }

                    // CENTRAL
                    for (var key in areas[0].AreaBoundary) {
                        cCoords.push({ lat: areas[0].AreaBoundary[key].Latitude, lng: areas[0].AreaBoundary[key].Longitude });
                    }

                    // SOUTH WEST
                    for (var key in areas[4].AreaBoundary) {
                        swCoords.push({ lat: areas[4].AreaBoundary[key].Latitude, lng: areas[4].AreaBoundary[key].Longitude });
                    }
                },
                error: function (data) {
                    alert("Error retrieving data. Please refresh the page.");
                }
            });

            updateTable();
            window.setInterval(updateMap, 5000); // Recursive call every 5 seconds
        });

        function updateMap() {
            getIncidents();
            updateTable();
        }

        function initMap() {
            map = new google.maps.Map(document.getElementById("map"), {
                center: { lat: 1.361942, lng: 103.821174 },
                zoom: 12
            });

            // Get incident and create markers
            getIncidents();

            // NORTH EAST
            nePolygon = new google.maps.Polygon({
                paths: neCoords,
                strokeColor: '#FF7DB8',
                strokeOpacity: 0.8,
                strokeWeight: 2,
                fillColor: '#FF7DB8',
                fillOpacity: 0.5
            });
            nePolygon.setMap(map);

            // NORTH WEST
            nwPolygon = new google.maps.Polygon({
                paths: nwCoords,
                strokeColor: '#A8DDAD',
                strokeOpacity: 0.8,
                strokeWeight: 2,
                fillColor: '#A8DDAD',
                fillOpacity: 0.5
            });
            nwPolygon.setMap(map);

            // SOUTH EAST
            sePolygon = new google.maps.Polygon({
                paths: seCoords,
                strokeColor: '#D4EFBF',
                strokeOpacity: 0.8,
                strokeWeight: 2,
                fillColor: '#D4EFBF',
                fillOpacity: 0.5
            });
            sePolygon.setMap(map);

            // CENTRAL
            cPolygon = new google.maps.Polygon({
                paths: cCoords,
                strokeColor: '#EAC97F',
                strokeOpacity: 0.8,
                strokeWeight: 2,
                fillColor: '#EAC97F',
                fillOpacity: 0.5
            });
            cPolygon.setMap(map);

            // SOUTH WEST
            swPolygon = new google.maps.Polygon({
                paths: swCoords,
                strokeColor: '#DB89EF',
                strokeOpacity: 0.8,
                strokeWeight: 2,
                fillColor: '#DB89EF',
                fillOpacity: 0.5
            });
            swPolygon.setMap(map);
        }

        function getIncidents() {
            // Remove markers from map
            for (var i = 0; i < markers.length; i++) {
                markers[i].setMap(null);
            }
            markers = [];

            $.ajax({
                type: "GET",
                url: api + "Nea/GetWeather",
                contentType: "application/json",
                async: false,
                success: function (data) {
                    weather = data;

                    for (var key in weather) {
                        var marker;
                        marker = new google.maps.Marker({
                            position: { lat: weather[key].Location.Latitude, lng: weather[key].Location.Longitude },
                            icon: weather[key].ImageUri
                        });
                        markers.push(marker);
                    }
                },
                error: function (data) {
                    alert("Error retrieving data. Please refresh the page.");
                }
            });

            $.ajax({
                type: "GET",
                url: api + "Incident/GetUnresolvedIncidents",
                contentType: "application/json",
                async: false,
                success: function (data) {
                    incidents = data;

                    for (var key in incidents) {
                        var marker;
                        switch (incidents[key].IncidentType.IncidentTypeId) {
                            case "AMBL":
                                marker = new google.maps.Marker({
                                    position: { lat: incidents[key].Location.Latitude, lng: incidents[key].Location.Longitude },
                                    icon: "http://maps.google.com/mapfiles/ms/icons/green.png"
                                });
                                break;
                            case "DENGUE":
                                marker = new google.maps.Marker({
                                    position: { lat: incidents[key].Location.Latitude, lng: incidents[key].Location.Longitude },
                                    icon: "http://maps.google.com/mapfiles/ms/icons/orange.png"
                                });
                                break;
                            case "EVAC":
                                marker = new google.maps.Marker({
                                    position: { lat: incidents[key].Location.Latitude, lng: incidents[key].Location.Longitude },
                                    icon: "http://maps.google.com/mapfiles/ms/icons/purple.png"
                                });
                                break;
                            case "FIRE":
                                marker = new google.maps.Marker({
                                    position: { lat: incidents[key].Location.Latitude, lng: incidents[key].Location.Longitude },
                                    icon: "http://maps.google.com/mapfiles/ms/icons/red.png"
                                });
                                break;
                            case "GAS":
                                marker = new google.maps.Marker({
                                    position: { lat: incidents[key].Location.Latitude, lng: incidents[key].Location.Longitude },
                                    icon: "http://maps.google.com/mapfiles/ms/icons/yellow.png"
                                });
                                break;
                            case "ZIKA":
                                marker = new google.maps.Marker({
                                    position: { lat: incidents[key].Location.Latitude, lng: incidents[key].Location.Longitude },
                                    icon: "http://maps.google.com/mapfiles/ms/icons/pink.png"
                                });
                                break;
                            case "HAZE":
                                marker = new google.maps.Marker({
                                    position: { lat: incidents[key].Location.Latitude, lng: incidents[key].Location.Longitude },
                                    icon: "http://maps.google.com/mapfiles/ms/icons/blue.png"
                                });
                                break;
                            default:
                                break;
                        }
                        markers.push(marker);
                    }
                },
                error: function (data) {
                    alert("Error retrieving data. Please refresh the page.");
                }
            });

            // Add markers to map
            for (var i = 0; i < markers.length; i++) {
                markers[i].setMap(map);
            }
        }

        function updateTable() {
            // NORTH EAST
            $.ajax({
                type: "GET",
                url: api + "IncidentType/GetIncidentTypesInArea?areaId=NE",
                contentType: "application/json",
                async: false,
                success: function (data) {
                    ne = data;
                },
                error: function (data) {
                    alert("Error retrieving data. Please refresh the page.");
                }
            });

            // NORTH WEST
            $.ajax({
                type: "GET",
                url: api + "IncidentType/GetIncidentTypesInArea?areaId=NW",
                contentType: "application/json",
                async: false,
                success: function (data) {
                    nw = data;
                },
                error: function (data) {
                    alert("Error retrieving data. Please refresh the page.");
                }
            });

            // SOUTH EAST
            $.ajax({
                type: "GET",
                url: api + "IncidentType/GetIncidentTypesInArea?areaId=SE",
                contentType: "application/json",
                async: false,
                success: function (data) {
                    se = data;
                },
                error: function (data) {
                    alert("Error retrieving data. Please refresh the page.");
                }
            });

            // CENTRAL
            $.ajax({
                type: "GET",
                url: api + "IncidentType/GetIncidentTypesInArea?areaId=C",
                contentType: "application/json",
                async: false,
                success: function (data) {
                    c = data;
                },
                error: function (data) {
                    alert("Error retrieving data. Please refresh the page.");
                }
            });

            // SOUTH WEST
            $.ajax({
                type: "GET",
                url: api + "IncidentType/GetIncidentTypesInArea?areaId=SW",
                contentType: "application/json",
                async: false,
                success: function (data) {
                    sw = data;
                },
                error: function (data) {
                    alert("Error retrieving data. Please refresh the page.");
                }
            });
            
            // ARRAY[0] AMBL, ARRAY[1] DENGUE, ARRAY[2] EVAC, ARRAY[3] FIRE, ARRAY[4] GAS, ARRAY [5] ZIKA, ARRAY[6] HAZE
            // TABLE DENGUE | ZIKA | HAZE | AMBULANCE | RESCUE & EVAC | FIRE-FIGHTING | GAS LEAK
            $("#table tbody").html(
                "<tr><th><div class='box ne'></div>North East</th>" +
                    "<td>" + ne[1].Severity + "</td>" +
                    "<td>" + ne[5].Severity + "</td>" +
                    "<td>" + ne[6].Severity + "</td>" +
                    "<td>" + ne[0].Severity + "</td>" +
                    "<td>" + ne[2].Severity + "</td>" +
                    "<td>" + ne[3].Severity + "</td>" +
                    "<td>" + ne[4].Severity + "</td></tr>" +
                "<tr><th><div class='box nw'></div>North West</th>" +
                    "<td>" + nw[1].Severity + "</td>" +
                    "<td>" + nw[5].Severity + "</td>" +
                    "<td>" + nw[6].Severity + "</td>" +
                    "<td>" + nw[0].Severity + "</td>" +
                    "<td>" + nw[2].Severity + "</td>" +
                    "<td>" + nw[3].Severity + "</td>" +
                    "<td>" + nw[4].Severity + "</td></tr>" +
                "<tr><th><div class='box se'></div>South East</th>" +
                    "<td>" + se[1].Severity + "</td>" +
                    "<td>" + se[5].Severity + "</td>" +
                    "<td>" + se[6].Severity + "</td>" +
                    "<td>" + se[0].Severity + "</td>" +
                    "<td>" + se[2].Severity + "</td>" +
                    "<td>" + se[3].Severity + "</td>" +
                    "<td>" + se[4].Severity + "</td></tr>" +
                "<tr><th><div class='box c'></div>Central</th>" +
                    "<td>" + c[1].Severity + "</td>" +
                    "<td>" + c[5].Severity + "</td>" +
                    "<td>" + c[6].Severity + "</td>" +
                    "<td>" + c[0].Severity + "</td>" +
                    "<td>" + c[2].Severity + "</td>" +
                    "<td>" + c[3].Severity + "</td>" +
                    "<td>" + c[4].Severity + "</td></tr>" +
                "<tr><th><div class='box sw'></div>South West</th>" +
                    "<td>" + sw[1].Severity + "</td>" +
                    "<td>" + sw[5].Severity + "</td>" +
                    "<td>" + sw[6].Severity + "</td>" +
                    "<td>" + sw[0].Severity + "</td>" +
                    "<td>" + sw[2].Severity + "</td>" +
                    "<td>" + sw[3].Severity + "</td>" +
                    "<td>" + sw[4].Severity + "</td></tr>");
        }
    </script>
    
    <script src="https://maps.googleapis.com/maps/api/js?key=AIzaSyAJtTxL3VYN0qnKjw86I3zoTApVUk43w6w&callback=initMap" async defer></script>
</asp:Content>
