/* chartjs (#purchase) */
function purchase(){
	var myCanvas = document.getElementById("purchase");
	myCanvas.height="260";

	var myCanvasContext = myCanvas.getContext("2d");
	var gradientStroke1 = myCanvasContext.createLinearGradient(0, 0, 0, 380);
	gradientStroke1.addColorStop(0, hexToRgba(myVarVal, 0.3) || 'rgba(236, 41, 107 ,0.3)');

	var gradientStroke2 = myCanvasContext.createLinearGradient(0, 0, 0, 280);
	gradientStroke2.addColorStop(0, hexToRgba(myVarVal1, 0.3) || 'rgba(72, 1, 255,0.3)');

    var myChart = new Chart( myCanvas, {
		type: 'line',
		data: {
            labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
            type: 'line',
            datasets: [ {
				label: 'Female',
				data: [16, 32, 18, 26, 42, 33, 44],
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
				label: "Male",
				data: [ 22, 44, 67, 43, 76, 45, 50],
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
}

/* echart (#echart5) */
function echart5(){
  var chartdata3 = [
    {
      name: 'Good',
      type: 'bar',
      stack: 'Stack',
      data: [20, 56, 18, 75, 65, 74, 78]
    },
    {
      name: 'Bad',
      type: 'bar',
      stack: 'Stack',
      data: [12, 14, 15, 50, 24, 24, 10]
    }
  ];

  var option5 = {
    grid: {
      top: '6',
      right: '0',
      bottom: '17',
      left: '25',
    },
	tooltip: {
		show: true,
		showContent: true,
		alwaysShowContent: true,
		triggerOn: 'mousemove',
		trigger: 'axis',
		axisPointer:
			{
				label: {
					show: false,
				}
			}
	},
    xAxis: {
      data: ['Mon', 'Tue', 'Wed', 'Thur', 'Fri', 'Sat', 'Sun'],
      axisLine: {
        lineStyle: {
          color: 'rgba(227, 237, 252,0.5)'
        }
      },
      axisLabel: {
        fontSize: 10,
        color: '#a7b4c9'
      }
    },
    yAxis: {
      splitLine: {
        lineStyle: {
          color: 'rgba(227, 237, 252,0.5)'
        }
      },
      axisLine: {
        lineStyle: {
          color: 'rgba(227, 237, 252,0.5)'
        }
      },
      axisLabel: {
        fontSize: 10,
        color: '#a7b4c9'
      }
    },
    series: chartdata3,
	color:[ myVarVal, myVarVal1],
	barMaxWidth: 20
  };

  var chart5 = document.getElementById('echart5');
  var barChart5 = echarts.init(chart5);
  barChart5.setOption(option5);
  window.addEventListener('resize',function(){
	barChart5.resize();
  })
}