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
        var legend;
        var areas;
        var incidents;
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
            $.ajax({
                type: "GET",
                url: api + "Incident/GetUnresolvedIncidents",
                contentType: "application/json",
                async: false,
                success: function (data) {
                    incidents = data;
                },
                error: function (data) {
                    alert("Error retrieving data. Please refresh the page.");
                }
            });

            for (var key in incidents) {
                switch (incidents[key].IncidentType.IncidentTypeId) {
                    case "AMBL":
                        var marker = new google.maps.Marker({
                            position: { lat: incidents[key].Location.Latitude, lng: incidents[key].Location.Longitude },
                            icon: "http://maps.google.com/mapfiles/ms/icons/green.png",
                            map: map
                        });
                        break;
                    case "DENGUE":
                        var marker = new google.maps.Marker({
                            position: { lat: incidents[key].Location.Latitude, lng: incidents[key].Location.Longitude },
                            icon: "http://maps.google.com/mapfiles/ms/icons/orange.png",
                            map: map
                        });
                        break;
                    case "EVAC":
                        var marker = new google.maps.Marker({
                            position: { lat: incidents[key].Location.Latitude, lng: incidents[key].Location.Longitude },
                            icon: "http://maps.google.com/mapfiles/ms/icons/purple.png",
                            map: map
                        });
                        break;
                    case "FIRE":
                        var marker = new google.maps.Marker({
                            position: { lat: incidents[key].Location.Latitude, lng: incidents[key].Location.Longitude },
                            icon: "http://maps.google.com/mapfiles/ms/icons/red.png",
                            map: map
                        });
                        break;
                    case "GAS":
                        var marker = new google.maps.Marker({
                            position: { lat: incidents[key].Location.Latitude, lng: incidents[key].Location.Longitude },
                            icon: "http://maps.google.com/mapfiles/ms/icons/yellow.png",
                            map: map
                        });
                        break;
                    case "ZIKA":
                        var marker = new google.maps.Marker({
                            position: { lat: incidents[key].Location.Latitude, lng: incidents[key].Location.Longitude },
                            icon: "http://maps.google.com/mapfiles/ms/icons/pink.png",
                            map: map
                        });
                        break;
                    case "HAZE":
                        var marker = new google.maps.Marker({
                            position: { lat: incidents[key].Location.Latitude, lng: incidents[key].Location.Longitude },
                            icon: "http://maps.google.com/mapfiles/ms/icons/blue.png",
                            map: map
                        });
                        break;
                    default:
                        break;
                }
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
                "<tr><th>North East</th>" +
                    "<td>" + ne[1].Severity + "</td>" +
                    "<td>" + ne[5].Severity + "</td>" +
                    "<td>" + ne[6].Severity + "</td>" +
                    "<td>" + ne[0].Severity + "</td>" +
                    "<td>" + ne[2].Severity + "</td>" +
                    "<td>" + ne[3].Severity + "</td>" +
                    "<td>" + ne[4].Severity + "</td></tr>" +
                "<tr><th>North West</th>" +
                    "<td>" + nw[1].Severity + "</td>" +
                    "<td>" + nw[5].Severity + "</td>" +
                    "<td>" + nw[6].Severity + "</td>" +
                    "<td>" + nw[0].Severity + "</td>" +
                    "<td>" + nw[2].Severity + "</td>" +
                    "<td>" + nw[3].Severity + "</td>" +
                    "<td>" + nw[4].Severity + "</td></tr>" +
                "<tr><th>South East</th>" +
                    "<td>" + se[1].Severity + "</td>" +
                    "<td>" + se[5].Severity + "</td>" +
                    "<td>" + se[6].Severity + "</td>" +
                    "<td>" + se[0].Severity + "</td>" +
                    "<td>" + se[2].Severity + "</td>" +
                    "<td>" + se[3].Severity + "</td>" +
                    "<td>" + se[4].Severity + "</td></tr>" +
                "<tr><th>Central</th>" +
                    "<td>" + c[1].Severity + "</td>" +
                    "<td>" + c[5].Severity + "</td>" +
                    "<td>" + c[6].Severity + "</td>" +
                    "<td>" + c[0].Severity + "</td>" +
                    "<td>" + c[2].Severity + "</td>" +
                    "<td>" + c[3].Severity + "</td>" +
                    "<td>" + c[4].Severity + "</td></tr>" +
                "<tr><th>South West</th>" +
                    "<td>" + sw[1].Severity + "</td>" +
                    "<td>" + sw[5].Severity + "</td>" +
                    "<td>" + sw[6].Severity + "</td>" +
                    "<td>" + sw[0].Severity + "</td>" +
                    "<td>" + sw[2].Severity + "</td>" +
                    "<td>" + sw[3].Severity + "</td>" +
                    "<td>" + sw[4].Severity + "</td></tr>");
        }

        function updateMap() {
            getIncidents();
            updateTable();
        }
    </script>
    
    <script src="https://maps.googleapis.com/maps/api/js?key=AIzaSyAJtTxL3VYN0qnKjw86I3zoTApVUk43w6w&callback=initMap" async defer></script>
</asp:Content>
