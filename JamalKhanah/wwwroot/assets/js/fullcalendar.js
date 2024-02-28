// sample calendar events data
'use strict'
var curYear = moment().format('YYYY');
var curMonth = moment().format('MM');
// Calendar Event Source
var sptCalendarEvents = {
  id: 1,
  events: [{
    id: '1',
    start: curYear + '-' + curMonth + '-02',
    end: curYear + '-' + curMonth + '-03',
    title: 'Spruko Meetup',
    backgroundColor: 'rgba(236, 41, 107, 0.7)',
    description: 'All the Lorem Ipsum generators on the Internet tend to repeat predefined chunks as necessary'
  }, {
    id: '2',
    start: curYear + '-' + curMonth + '-17',
    end: curYear + '-' + curMonth + '-17',
    title: 'Design Review',
    backgroundColor: 'rgba(68, 86, 195, 0.7)',
    description: 'All the Lorem Ipsum generators on the Internet tend to repeat predefined chunks as necessary'
  }, {
    id: '3',
    start: curYear + '-' + curMonth + '-13',
    end: curYear + '-' + curMonth + '-13',
    title: 'Lifestyle Conference',
    backgroundColor: 'rgba(236, 41, 107, 0.7)',
    description: 'All the Lorem Ipsum generators on the Internet tend to repeat predefined chunks as necessary'
  }, {
    id: '4',
    start: curYear + '-' + curMonth + '-21',
    end: curYear + '-' + curMonth + '-21',
    title: 'Team Weekly Brownbag',
    backgroundColor: 'rgba(3, 115, 242, 0.7)',
    description: 'All the Lorem Ipsum generators on the Internet tend to repeat predefined chunks as necessary'
  }, {
    id: '5',
    start: curYear + '-' + curMonth + '-04T10:00:00',
    end: curYear + '-' + curMonth + '-06T15:00:00',
    title: 'Music Festival',
    backgroundColor: 'rgba(253, 96, 116, 0.7)',
    description: 'All the Lorem Ipsum generators on the Internet tend to repeat predefined chunks as necessary'
  }, {
    id: '6',
    start: curYear + '-' + curMonth + '-23T13:00:00',
    end: curYear + '-' + curMonth + '-25T18:30:00',
    title: 'Attend Lea\'s Wedding',
    backgroundColor: 'rgba(253, 96, 116, 0.7)',
    description: 'All the Lorem Ipsum generators on the Internet tend to repeat predefined chunks as necessary'
  }]
};
// Birthday Events Source
var sptBirthdayEvents = {
  id: 2,
  backgroundColor: 'rgba(33, 182, 50, 0.7)',
  textColor: '#fff',
  events: [{
    id: '7',
    start: curYear + '-' + curMonth + '-04',
    end: curYear + '-' + curMonth + '-04',
    title: 'Harcates Birthday',
    description: 'All the Lorem Ipsum generators on the Internet tend to repeat predefined chunks as necessary'
  }, {
    id: '8',
    start: curYear + '-' + curMonth + '-28',
    end: curYear + '-' + curMonth + '-28',
    title: 'Jinnysin\'s Birthday',
    description: 'All the Lorem Ipsum generators on the Internet tend to repeat predefined chunks as necessary'
  }, {
    id: '9',
    start: curYear + '-' + curMonth + '-31',
    end: curYear + '-' + curMonth + '-31',
    title: 'Lee shin\'s Birthday',
    description: 'All the Lorem Ipsum generators on the Internet tend to repeat predefined chunks as necessary'
  }]
};
var sptHolidayEvents = {
  id: 3,
  backgroundColor: 'rgba(4, 178, 229, 0.7)',
  textColor: '#fff',
  events: [{
    id: '10',
    start: curYear + '-' + curMonth + '-05',
    end: curYear + '-' + curMonth + '-08',
    title: 'Festival Day'
  }, {
    id: '11',
    start: curYear + '-' + curMonth + '-18',
    end: curYear + '-' + curMonth + '-19',
    title: 'Memorial Day'
  }, {
    id: '12',
    start: curYear + '-' + curMonth + '-25',
    end: curYear + '-' + curMonth + '-26',
    title: 'Diwali'
  }]
};
var sptOtherEvents = {
  id: 4,
  backgroundColor: 'rgba(251, 149, 5, 0.7)',
  textColor: '#fff',
  events: [{
    id: '13',
    start: curYear + '-' + curMonth + '-07',
    end: curYear + '-' + curMonth + '-09',
    title: 'My Rest Day'
  }, {
    id: '13',
    start: curYear + '-' + curMonth + '-29',
    end: curYear + '-' + curMonth + '-31',
    title: 'My Rest Day'
  }]
};