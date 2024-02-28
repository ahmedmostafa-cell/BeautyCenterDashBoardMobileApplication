function lineChart() {
	/* line chart */

	var myCanvas = document.getElementById("lineChart");
	// myCanvas.height="350";
	myCanvas.innerHTML = "";
	var myCanvasContext = myCanvas.getContext("2d");
	var gradientStroke1 = myCanvasContext.createLinearGradient(0, 0, 0, 380);
	gradientStroke1.addColorStop(0, hexToRgba(myVarVal, 0.3) || 'rgb(236, 41, 107,0.3)');

	var gradientStroke2 = myCanvasContext.createLinearGradient(0, 0, 0, 280);
	gradientStroke2.addColorStop(0, hexToRgba(myVarVal1, 0.3) || 'rgb(72, 1, 255,0.3)');

    var myChart = new Chart( myCanvas, {
		type: 'line',
		data: {
            labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'June', 'July'],
            type: 'line',
            datasets: [ {
				label: 'Total Collection',
				data: [0, 70, 75, 120, 94, 141, 60],
				backgroundColor: gradientStroke1,
				borderColor: myVarVal,
				pointBackgroundColor: myVarVal,
				pointHoverBackgroundColor:gradientStroke1,
				pointBorderColor : myVarVal,
				pointHoverBorderColor :gradientStroke1,
				pointBorderWidth :3,
				pointRadius :3,
				pointHoverRadius :2,
				borderWidth: 2
            }, {
				label: "Fees Collection",
				data: [0, 50, 40, 80, 40, 79, 50],
				backgroundColor: gradientStroke2,
				borderColor: myVarVal1,
				pointBackgroundColor: myVarVal1,
				pointHoverBackgroundColor:gradientStroke2,
				pointBorderColor : myVarVal1,
				pointHoverBorderColor :gradientStroke2,
				pointBorderWidth :3,
				pointRadius :3,
				pointHoverRadius :2,
				borderWidth: 2
			}
			]
        },
		options: {
			responsive: true,
			maintainAspectRatio: false,
			tooltips: {
				mode: 'index',
				titleFontSize: 12,
				titleFontColor: '#000',
				bodyFontColor: '#000',
				backgroundColor: '#fff',
				cornerRadius: 3,
				intersect: false,
			},
			legend: {
				display: true,
				labels: {
					usePointStyle: false,
					fontColor: "#b8b9bd"
				},
			},
			scales: {
				xAxes: [{
					ticks: {
						fontColor: "#b8b9bd",
					 },
					display: true,
					gridLines: {
						display: true,
						color:'rgba(96, 94, 126, 0.1)',
						drawBorder: false
					},
					scaleLabel: {
						display: false,
						labelString: 'Month',
						fontColor: 'transparent'
					}
				}],
				yAxes: [{
					ticks: {
						fontColor: "#b8b9bd",
					 },
					display: true,
					gridLines: {
						display: true,
						color:'rgba(96, 94, 126, 0.1)',
						drawBorder: false
					},
					scaleLabel: {
						display: false,
						labelString: 'sales',
						fontColor: 'transparent'
					}
				}]
			},
			title: {
				display: false,
				text: 'Normal Legend'
			}
		}
	});
	/* line chart end */

	$(".sparkline_bar-2").sparkline([6,2,8,4,3,8,1,3,6,5,7], {
		type: 'bar',
		height: 90,
		colorMap: {
			'9': '#a1a1a1'
		},
		barColor: myVarVal1,
		barSpacing: 7,
		barWidth: 6,
	});
	/* sparkline_bar end */
}