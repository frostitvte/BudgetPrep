<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="BudgetPrep._Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <style type="text/css">
        .first-column {
          width: 48%;
          float: left;
          padding-left: 33px;
        }

        .second-column {
          width: 48%;
          float: right;
          padding-right: 33px;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row-fluid">
        <div id="dashboard_links" class="col-xs-12 col-sm-2 pull-right">
            <ul class="nav nav-pills nav-stacked">
                <li class="active"><a href="#" class="tab-link" id="Mengurus">Mengurus</a></li>
                <li><a href="#" class="tab-link" id="Perjawatan">Perjawatan</a></li>
                <li><a href="#" class="tab-link" id="Projek">Projek</a></li>
            </ul>
        </div>
        <div id="dashboard_tabs" class="col-xs-12 col-sm-10"">
            <!--Mengurus-->
            <div id="dashboard-Mengurus" class="row userTab" style="visibility: visible; position: relative;">
                <div id="box-Mengurus" class="box-content">
                    <div class="form-horizontal" role="form">
                        <div class="form-group">
                           <!--tab mengurus start-->
                           <div class="label label-default" style="margin-right:30px;font-size:15px;float:right;">Budget Mengurus</div><br /><br />
                           <div id="row1-mengurus" class="row">
                               <div class="first-column">
                                   <div class="col-lg-15 col-sm-15">
                                       <div class="panel panel-primary">
                                           <div class="panel-heading">5 Years Analysis - Approved Budget VS Actual Expenses</div>
                                           <div class="panel-body">
                                               <div id="chartContainerMengurusAPvsAE" style="height: 300px; width: 100%;"></div>
                                           </div>
                                       </div>
                                    </div>
                               </div>

                               <div class="second-column">
                                    <div class="col-lg-15 col-sm-15">
                                       <div class="panel panel-primary">
                                           <div class="panel-heading">5 Years Analysis - Approved Budget VS Amended Budget</div>
                                           <div class="panel-body">
                                               <div id="chartContainerMengurusAPvsAB" style="height: 300px; width: 100%;"></div>
                                           </div>
                                       </div>
                                    </div>
                                </div>
                            </div>
      
                            <div id="row2-mengurus" class="row">
                               <div class="first-column">
                                   <div class="col-lg-15 col-sm-15">
                                       <div class="panel panel-primary">
                                           <div class="panel-heading">5 Years Analysis - Approved Budget</div>
                                           <div class="panel-body">
                                               <div id="chartContainerMengurusYear" style="height: 300px; width: 100%;"></div>
                                           </div>
                                       </div>
                                    </div>
                               </div>

                               <div class="second-column">
                                    <div class="col-lg-15 col-sm-15">
                                       <div class="panel panel-primary">
                                           <div class="panel-heading">5 Years Analysis - Actual Expenses</div>
                                           <div class="panel-body">
                                               <div id="chartContainerMengurusAccount" style="height: 300px; width: 100%;"></div>
                                           </div>
                                       </div>
                                    </div>
                                </div>
                            </div>
                            <!--tab mengurus end-->
                        </div>
                    </div>
                </div>
            </div>
            <!--Mengurus-->

            <!--Perjawatan-->
            <div id="dashboard-Perjawatan" class="row userTab" style="visibility: hidden; display: none; position: relative;">
                <div id="box-Perjawatan" class="box-content">
                    <div class="form-horizontal" role="form">
                        <div class="form-group">
                            <!--tab perjawatan start-->
                            <div class="label label-default" style="margin-right:30px;font-size:15px;float:right;">Budget Perjawatan</div><br /><br />
                            <div id="Div1" class="row">
                               <div class="first-column">
                                   <div class="col-lg-15 col-sm-15">
                                       <div class="panel panel-primary">
                                           <div class="panel-heading">To Be Confirm</div>
                                           <div class="panel-body">
                                               <div id="Div4" style="height: 300px; width: 100%;"></div>
                                           </div>
                                       </div>
                                    </div>
                               </div>

                               <div class="second-column">
                                    <div class="col-lg-15 col-sm-15">
                                       <div class="panel panel-primary">
                                           <div class="panel-heading">To Be Confirm</div>
                                           <div class="panel-body">
                                               <div id="Div5" style="height: 300px; width: 100%;"></div>
                                           </div>
                                       </div>
                                    </div>
                                </div>
                            </div>
                            <!--tab perjawatan end-->
                        </div>
                    </div>
                </div>
            </div>
            <!--Perjawatan-->

            <!--Projek-->
            <div id="dashboard-Projek" class="row userTab" style="visibility: hidden; display: none; position: relative; ">
                <div id="box-Projek" class="box-content">
                    <div class="form-horizontal" role="form">
                        <div class="form-group">
                            <!--tab projek start-->
                           <div class="label label-default" style="margin-right:30px;font-size:15px;float:right;">Budget Projek</div><br /><br />
                           <div id="row1-projek" class="row">
                               <div class="first-column">
                                   <div class="col-lg-15 col-sm-15">
                                       <div class="panel panel-primary">
                                           <div class="panel-heading">5 Years Analysis - Approved Budget VS Actual Expenses</div>
                                           <div class="panel-body">
                                               <div id="tba" style="height: 300px; width: 100%;"></div>
                                           </div>
                                       </div>
                                    </div>
                               </div>

                               <div class="second-column">
                                    <div class="col-lg-15 col-sm-15">
                                       <div class="panel panel-primary">
                                           <div class="panel-heading">5 Years Analysis - Approved Budget VS Amended Budget</div>
                                           <div class="panel-body">
                                               <div id="Div3" style="height: 300px; width: 100%;"></div>
                                           </div>
                                       </div>
                                    </div>
                                </div>
                            </div>
      
                            <div id="row2-projek" class="row">
                               <div class="first-column">
                                   <div class="col-lg-15 col-sm-15">
                                       <div class="panel panel-primary">
                                           <div class="panel-heading">5 Years Analysis - Approved Budget</div>
                                           <div class="panel-body">
                                               <div id="chartContainerProjekYear" style="height: 300px; width: 100%;"></div>
                                           </div>
                                       </div>
                                    </div>
                               </div>

                               <div class="second-column">
                                    <div class="col-lg-15 col-sm-15">
                                       <div class="panel panel-primary">
                                           <div class="panel-heading">5 Years Analysis - Actual Expenses</div>
                                           <div class="panel-body">
                                               <div id="Div6" style="height: 300px; width: 100%;"></div>
                                           </div>
                                       </div>
                                    </div>
                                </div>
                            </div>
                            <!--tab projek end-->
                        </div>
                    </div>
                </div>
            </div>
            <!--Projek-->
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContent" runat="server">
    <script src="js/canvasjs.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            //debugger;
            DashboardTabChecker();
        });

        window.onload = function () {
            //Mengurus
            MengurusYearData();
            MengurusAPvsAE();
            MengurusAPvsAB();
            //Projek
            ProjekYearData();
        }

        function MengurusYearData() {
            //mengurus
            $.ajax({
                error: function (msg) { debugger; alert(msg.d); },
                type: "Post",
                url: "Default.aspx/GetMengurusYearData",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (datas) {
                    var yourDataPoints = [];
                    var points = eval(datas.d);
                    $.each(points, function (index, Info) {
                        yourDataPoints.push({
                            y: Info.y,
                            label: Info.label,
                            color: Info.color
                        });
                    });
                    var chart = new CanvasJS.Chart("chartContainerMengurusYear",
                    {
                        height: 300,
                        axisY: { title: "Amounts(RM)", titleFontSize: 15 },
                        theme: "theme2",
                        zoomEnabled: true, interactivityEnabled: true, exportEnabled: true, animationEnabled: true,
                        data: [{
                            type: "column",
                            dataPoints: yourDataPoints,
                            click: function (e) {
                                DrawMengurusField(e.dataPoint.indexLabel)
                            },
                        }]
                    });

                    chart.render();
                },
                failure: function (msg) {
                    alert(msg.d);
                }
            });
        }

        function ProjekYearData() {
            //projek
            $.ajax({
                error: function (msg) { debugger; alert(msg.d); },
                type: "Post",
                url: "Default.aspx/GetProjekYearData",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (datas) {
                    var yourDataPoints = [];
                    var points = eval(datas.d);
                    $.each(points, function (index, Info) {
                        yourDataPoints.push({
                            y: Info.y,
                            label: Info.label,
                            color: Info.color
                        });
                    });
                    var chart = new CanvasJS.Chart("chartContainerProjekYear",
                    {
                        height: 300,
                        width: 387,
                        axisY: { title: "Amounts(RM)", titleFontSize: 15, },
                        theme: "theme2",
                        zoomEnabled: true, interactivityEnabled: true, exportEnabled: true, animationEnabled: true,
                        data: [{
                            type: "column",
                            dataPoints: yourDataPoints,
                        }]
                    });
                    chart.render();
                },
                failure: function (msg) {
                    alert(msg.d);
                }
            });
        }

        function MengurusAPvsAE() {
            $.ajax({
                error: function (msg) { debugger; alert(msg.d); },
                type: "Post",
                url: "Default.aspx/GetMengurusAPvsAE",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (datas) {
                    var dt = eval(datas.d);
                    var yourDataPoints1 = [];
                    var yourDataPoints2 = [];

                    //console.log(dt);

                    $.each(dt, function (i, data) {
                        yourDataPoints1.push({
                            y: parseFloat(data[0].SumBudget),
                            label: parseInt(data[0].Year),
                        });

                        yourDataPoints2.push({
                            y: parseFloat(data[1].SumBudget),
                            label: parseInt(data[1].Year)
                        });
                    });

                    var chart = new CanvasJS.Chart("chartContainerMengurusAPvsAE");

                    chart.options.axisY = { title: "Amounts(RM)", titleFontSize: 15, };
                    chart.options.theme = "theme2";
                    chart.options.legend = {
                        cursor: "pointer",
                        itemclick: function (e) {
                            if (typeof (e.dataSeries.visible) === "undefined" || e.dataSeries.visible) {
                                e.dataSeries.visible = false;
                            }
                            else {
                                e.dataSeries.visible = true;
                            }
                            chart.render();
                        }
                    };
                    chart.options.toolTip = {
                        shared: true,
                        content: function (e) {
                            var str = '';
                            var total = 0;
                            var str3;
                            var str2;
                            for (var i = 0; i < e.entries.length; i++) {
                                var str1 = "<span style= 'color:" + e.entries[i].dataSeries.color + "'> " + e.entries[i].dataSeries.name + "</span>: <strong>" + e.entries[i].dataPoint.y + "</strong> <br/>";
                                total = e.entries[i].dataPoint.y + total;
                                str = str.concat(str1);
                            }
                            str2 = "<span style = 'color:DodgerBlue; '><strong>" + e.entries[0].dataPoint.label + "</strong></span><br/>";
                            str3 = "<span style = 'color:Tomato '>Total: </span><strong>" + total + "</strong><br/>";

                            return (str2.concat(str)).concat(str3);
                        }

                    };

                    var series1 = { //dataSeries - first quarter
                        type: "bar",
                        name: "Approved Budget",
                        showInLegend: true
                    };

                    var series2 = { //dataSeries - second quarter
                        type: "bar",
                        name: "Actual Expenses",
                        showInLegend: true
                    };

                    chart.options.data = [];
                    chart.options.data.push(series1);
                    chart.options.data.push(series2);

                    series1.dataPoints = yourDataPoints1;

                    series2.dataPoints = yourDataPoints2;

                    chart.render();
                }
            });
        }

        //function ProjekAPvsAE()

        function MengurusAPvsAB() {
            $.ajax({
                error: function (msg) { debugger; alert(msg.d); },
                type: "Post",
                url: "Default.aspx/GetMengurusAPvsAB",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (datas) {
                    var dt = eval(datas.d);
                    var yourDataPoints = [];
                    var series = [];
                    var yourDataPoints1 = [];
                    var yourDataPoints2 = [];
                    var yourDataPoints3 = [];

                    for (var i = 0; i < dt.length; i++)
                    {
                        var dat = dt[i];
                        for (var j = 0; j < dat.length; j++)
                        {
                            if (i == 0) {
                                yourDataPoints1.push({
                                    y: parseFloat(dat[j].SumBudget),
                                    label: parseInt(dat[j].Year)
                                });
                            }
                            else if (i == 1) {
                                yourDataPoints2.push({
                                    y: parseFloat(dat[j].SumBudget),
                                    label: parseInt(dat[j].Year)
                                });
                            }
                            else if (i == 2) {
                                yourDataPoints3.push({
                                    y: parseFloat(dat[j].SumBudget),
                                    label: parseInt(dat[j].Year)
                                });
                            }
                        }
                    }

                    var chart = new CanvasJS.Chart("chartContainerMengurusAPvsAB",
                       {
                           animationEnabled: true,
                           toolTip: {
                               shared: true,
                               content: function (e) {
                                   var str = '';
                                   var total = 0;
                                   var str3;
                                   var str2;
                                   for (var i = 0; i < e.entries.length; i++) {
                                       var str1 = "<span style= 'color:" + e.entries[i].dataSeries.color + "'> " + e.entries[i].dataSeries.name + "</span>: $<strong>" + e.entries[i].dataPoint.y + "</strong>  bn<br/>";
                                       total = e.entries[i].dataPoint.y + total;
                                       str = str.concat(str1);
                                   }
                                   str2 = "<span style = 'color:DodgerBlue; '><strong>" + (e.entries[0].dataPoint.label) + "</strong></span><br/>";
                                   total = Math.round(total * 100) / 100
                                   str3 = "<span style = 'color:Tomato '>Total:</span><strong> $" + total + "</strong> bn<br/>";

                                   return (str2.concat(str)).concat(str3);
                               }
                           },
                           axisY: {
                               valueFormatString: ("#,##0.##"),
                               interval: 10,
                               interlacedColor: "rgba(182,177,168,0.2)"

                           },
                           axisX: {
                               interval: 1,
                               intervalType: "year"
                           },
                           data:
                               [
                               {
                                   //series 1
                                   type: "stackedColumn",       
                                   showInLegend:true,
                                   color: "#696661",
                                   name:"Q1",
                                   dataPoints: yourDataPoints1
                               },
                               {
                                   //series 2
                                   type: "stackedColumn",       
                                   showInLegend:true,
                                   name:"Q2",
                                   color: "#EDCA93",
                                   dataPoints: yourDataPoints3
                               },
                               {
                                   //series 3
                                   type: "stackedColumn",
                                   showInLegend: true,
                                   name: "Q3",
                                   color: "#275A42",
                                   dataPoints: yourDataPoints2
                               }
                               ]
                       });

                    chart.render();
                }
            });
        }

        function DrawMengurusField(valYear) {
            $.ajax({
                error: function (msg) { debugger; alert(msg.d); },
                type: "Post",
                url: "Default.aspx/GetMengurusFieldData",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({ Year: valYear }),
                dataType: "json",
                async: false,
                success: function (datas) {
                    var yourDataPoints = [];
                    var points = eval(datas.d);
                    $.each(points, function (index, Info) {
                        yourDataPoints.push({
                            y: Info.y,
                            legendText: Info.legendText,
                            indexLabel: Info.indexLabel,
                            color: Info.color
                        });
                    });
                    var chart = new CanvasJS.Chart("chartContainerMengurusField",
                    {
                        title: { text: "Field Budget ( " + valYear + " )"},
                        zoomEnabled: true, interactivityEnabled: true, exportEnabled: true, animationEnabled: true,
                        data: [{
                            type: "doughnut",
                            showInLegend: true,
                            dataPoints: yourDataPoints,
                            click: function (e) {
                                DrawMengurusAccount(valYear,e.dataPoint.indexLabel)
                            },
                        }]
                    });
                    chart.render();
                },
                failure: function (msg) {
                    alert(msg.d);
                }
            });
        }

        function DrawMengurusAccount(valYear,valField) {
            $.ajax({
                error: function (msg) { debugger; alert(msg.d); },
                type: "Post",
                url: "Default.aspx/GetMengurusAcountData",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({ Year: valYear, Field: valField }),
                dataType: "json",
                async: false,
                success: function (datas) {
                    var yourDataPoints = [];
                    var points = eval(datas.d);
                    $.each(points, function (index, Info) {
                        yourDataPoints.push({
                            y: Info.y,
                            legendText: Info.legendText,
                            label: Info.label,
                            color: Info.color
                        });
                    });
                    var chart = new CanvasJS.Chart("chartContainerMengurusAccount",
                    {
                        title: { text: "Account Budget ( " + valYear + ", " + valField + " )" },
                        zoomEnabled: true, exportEnabled: true, animationEnabled: true,
                        data: [{
                            type: "column",
                            showInLegend: false,
                            dataPoints: yourDataPoints,
                        }]
                    });
                    chart.render();
                },
                failure: function (msg) {
                    alert(msg.d);
                }
            });
        }

    </script>
</asp:Content>
