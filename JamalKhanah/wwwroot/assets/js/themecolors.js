const handleThemeUpdate = (cssVars) => {
    const root = document.querySelector(':root');
    const keys = Object.keys(cssVars);
    keys.forEach(key => {
        root.style.setProperty(key, cssVars[key]);
    });
}

function themeSwitch(switchProperty) {
    switchProperty.forEach((item) => {
        item.addEventListener('click', (e) => {
            const primaryColor = e.target.getAttribute('data-bg-color')
            const primaryHoverColor = e.target.getAttribute('data-bg-hover')
            const primaryBorderColor = e.target.getAttribute('data-bg-border')
            const primaryTransparent = e.target.getAttribute('data-bg-transparent')
            const secondaryColor = e.target.getAttribute('bg-color2')
            const secondaryHoverColor = e.target.getAttribute('bg-hover2')
            const secondaryBorderColor = e.target.getAttribute('bg-border2')

            handleThemeUpdate({
                '--primary-bg-color': primaryColor,
                '--primary-bg-hover': primaryHoverColor,
                '--primary-bg-border': primaryBorderColor,
                '--primary-transparentcolor': primaryTransparent,
                '--secondary': secondaryColor,
                '--secondary-hover': secondaryHoverColor,
                '--secondary-border': secondaryBorderColor,
            });

            $("input.input-color-picker[data-id='bg-color']").val(primaryColor)
            $("input.input-color-picker[data-id1='bg-hover']").val(primaryColor)
            $("input.input-color-picker[data-id2='bg-border']").val(primaryColor)
            $("input.input-color-picker[data-id7='transparentcolor']").val(primaryColor)
            $("input-secondary-color-picker[data-id5='bg-color2']").val(secondaryColor)
            $("input-secondary-color-picker[data-id6='bg-hover2']").val(secondaryColor)
            $("input-secondary-color-picker[data-id8='bg-border2']").val(secondaryColor)
        });
    });
}

function dynamicPrimaryColor(primaryColor) {
    primaryColor.forEach((item) => {
        item.addEventListener('input', (e) => {
            const cssPropName = `--primary-${e.target.getAttribute('data-id')}`;
            const cssPropName1 = `--primary-${e.target.getAttribute('data-id1')}`;
            const cssPropName2 = `--primary-${e.target.getAttribute('data-id2')}`;
            const cssPropName7 = `--primary-${e.target.getAttribute('data-id7')}`;
            handleThemeUpdate({
                [cssPropName]: e.target.value,
                // 95 is used as the opacity 0.95
                [cssPropName1]: e.target.value + 95,
                [cssPropName2]: e.target.value,
                [cssPropName7]: e.target.value + 20,
            });
        });
    });
}

function dynamicSecondaryColor(secondaryColor) {
    secondaryColor.forEach((item) => {
        item.addEventListener('input', (e) => {
            const cssPropName11 = `--secondary-${e.target.getAttribute('data-id5')}`;
            const cssPropName12 = `--secondary-${e.target.getAttribute('data-id6')}`;
            const cssPropName13 = `--secondary-${e.target.getAttribute('data-id8')}`;
            handleThemeUpdate({
                [cssPropName11]: e.target.value,
                // 95 is used as the opacity 0.95
                [cssPropName12]: e.target.value + 95,
                [cssPropName13]: e.target.value,
            });
        });
    });
}

function dynamicDarkPrimaryColor(darkprimaryColor) {
    darkprimaryColor.forEach((item) => {
        item.addEventListener('input', (e) => {
            const cssPropName = `--primary-${e.target.getAttribute('data-id')}`;
            const cssPropName1 = `--primary-${e.target.getAttribute('data-id1')}`;
            const cssPropName2 = `--primary-${e.target.getAttribute('data-id2')}`;
            handleThemeUpdate({
                [cssPropName]: e.target.value,
                // 95 is used as the opacity 0.95
                [cssPropName1]: e.target.value + 95,
                [cssPropName2]: e.target.value,
            });
        });
    });
}

function dynamicDarkSecondaryColor(darksecondaryColor) {
    darksecondaryColor.forEach((item) => {
        item.addEventListener('input', (e) => {
            const cssPropName11 = `--secondary-${e.target.getAttribute('data-id5')}`;
            const cssPropName12 = `--secondary-${e.target.getAttribute('data-id6')}`;
            const cssPropName13 = `--secondary-${e.target.getAttribute('data-id8')}`;
            handleThemeUpdate({
                [cssPropName11]: e.target.value,
                // 95 is used as the opacity 0.95
                [cssPropName12]: e.target.value + 95,
                [cssPropName13]: e.target.value,
            });
        });
    });
}

function dynamicTransPrimaryColor(transprimaryColor) {
    transprimaryColor.forEach((item) => {
        item.addEventListener('input', (e) => {
            const cssPropName = `--primary-${e.target.getAttribute('data-id')}`;
            const cssPropName1 = `--primary-${e.target.getAttribute('data-id1')}`;
            const cssPropName2 = `--primary-${e.target.getAttribute('data-id2')}`;
            const cssPropName7 = `--primary-${e.target.getAttribute('data-id7')}`;
            handleThemeUpdate({
                [cssPropName]: e.target.value,
                // 95 is used as the opacity 0.95
                [cssPropName1]: e.target.value + 95,
                [cssPropName2]: e.target.value,
                [cssPropName7]: e.target.value + 20,
            });
        });
    });
}

function dynamicTransSecondaryColor(transsecondaryColor) {
    transsecondaryColor.forEach((item) => {
        item.addEventListener('input', (e) => {
            const cssPropName11 = `--secondary-${e.target.getAttribute('data-id5')}`;
            const cssPropName12 = `--secondary-${e.target.getAttribute('data-id6')}`;
            const cssPropName13 = `--secondary-${e.target.getAttribute('data-id8')}`;
            handleThemeUpdate({
                [cssPropName11]: e.target.value,
                // 95 is used as the opacity 0.95
                [cssPropName12]: e.target.value + 95,
                [cssPropName13]: e.target.value + 20,
            });
        });
    });
}

function dynamicTransBackgroundColor(transbackgroundColor) {
    transbackgroundColor.forEach((item) => {
        item.addEventListener('input', (e) => {
            const cssPropName12 = `--transparent-${e.target.getAttribute('data-id9')}`;
            handleThemeUpdate({
                [cssPropName12]: e.target.value,
            });
        });
    });
}

$(document).ready(function() {
    // Light theme color picker
    const LightThemeSwitchers = document.querySelectorAll('.light-theme .switch_section span');
    const dynamicPrimaryLight = document.querySelectorAll('input.color-primary-light');
    const dynamicSecondaryLight = document.querySelectorAll('input.color-secondary-light');
    const dynamicPrimaryDarkColor = document.querySelectorAll('input.color-primary-dark');
    const dynamicSecondaryDark = document.querySelectorAll('input.color-secondary-dark');
    const dynamicPrimaryTransColor = document.querySelectorAll('input.color-primary-Transparent');
    const dynamicSecondaryTrans = document.querySelectorAll('input.color-secondary-Transparent');
    const dynamicBackgroundTrans = document.querySelectorAll('input.bg-primary-Transparent');

    themeSwitch(LightThemeSwitchers);
    dynamicPrimaryColor(dynamicPrimaryLight);
    dynamicSecondaryColor(dynamicSecondaryLight);
    dynamicDarkPrimaryColor(dynamicPrimaryDarkColor);
    dynamicDarkSecondaryColor(dynamicSecondaryDark);
    dynamicTransPrimaryColor(dynamicPrimaryTransColor);
    dynamicTransSecondaryColor(dynamicSecondaryTrans);
    dynamicTransBackgroundColor(dynamicBackgroundTrans);

    localStorageBackup();
});

function localStorageBackup() {
    // if there is a value stored, update color picker and background color
    // Used to retrive the data from local storage
    if (localStorage.primaryColor) {
        // document.getElementById('colorID').value = localStorage.primaryColor;
        document.querySelector('html').style.setProperty('--primary-bg-color', localStorage.primaryColor);
        document.querySelector('html').style.setProperty('--primary-bg-hover', localStorage.primaryHoverColor);
        document.querySelector('html').style.setProperty('--primary-bg-border', localStorage.primaryBorderColor);
        document.querySelector('html').style.setProperty('--primary-transparentcolor', localStorage.primaryTransparent);
        document.querySelector('body').classList.remove('dark-theme');
        document.querySelector('body').classList.remove('transparent-theme');
        document.querySelector('body').classList.add('light-theme');

        $('#myonoffswitch56').prop('checked', true);
    }
    if (localStorage.secondaryColor) {
        // document.getElementById('colorID2').value = localStorage.secondaryColor;
        document.querySelector('html').style.setProperty('--secondary-color', localStorage.secondaryColor);
        document.querySelector('html').style.setProperty('--secondary-hover', localStorage.secondaryHoverColor);
        document.querySelector('html').style.setProperty('--secondary-border', localStorage.secondaryBorderColor);
        document.querySelector('body').classList.remove('dark-theme');
        document.querySelector('body').classList.remove('transparent-theme');
        document.querySelector('body').classList.add('light-theme');

        $('#myonoffswitch56').prop('checked', true);
    }

    if (localStorage.darkPrimary) {
        // document.getElementById('darkPrimaryColorID').value = localStorage.darkPrimary;
        document.querySelector('html').style.setProperty('--primary-bg-color', localStorage.darkPrimary);
        document.querySelector('html').style.setProperty('--primary-bg-hover', localStorage.darkPrimary);
        document.querySelector('html').style.setProperty('--primary-bg-border', localStorage.darkPrimary);
        document.querySelector('body').classList.add('dark-theme');
        document.querySelector('body').classList.remove('light-theme');
        document.querySelector('body').classList.remove('transparent-theme');

        $('#myonoffswitch57').prop('checked', true);
    }
    if (localStorage.darksecondary) {
        // document.getElementById('darkColorID2').value = localStorage.darksecondary;
        document.querySelector('html').style.setProperty('--secondary-color', localStorage.darksecondary);
        document.querySelector('html').style.setProperty('--secondary-hover', localStorage.darksecondary);
        document.querySelector('html').style.setProperty('--secondary-border', localStorage.darksecondary);
        document.querySelector('body').classList.add('dark-theme');
        document.querySelector('body').classList.remove('light-theme');
        document.querySelector('body').classList.remove('transparent-theme');

        $('#myonoffswitch57').prop('checked', true);
    }

    if (localStorage.transparentPrimary) {
        // document.getElementById('TransparentPrimaryColorID').value = localStorage.transparentPrimary;
        document.querySelector('html').style.setProperty('--primary-bg-color', localStorage.transparentPrimary);
        document.querySelector('html').style.setProperty('--primary-bg-hover', localStorage.primaryHoverColor);
        document.querySelector('html').style.setProperty('--primary-bg-border', localStorage.primaryBorderColor);
        document.querySelector('body').classList.remove('dark-theme');
        document.querySelector('body').classList.remove('light-theme');
        document.querySelector('body').classList.add('transparent-theme');

        $('#myonoffswitch58').prop('checked', true);
    }
    if (localStorage.transparentsecondary) {
        // document.getElementById('TransparentColorID2').value = localStorage.transparentsecondary;
        document.querySelector('html').style.setProperty('--secondary-color', localStorage.transparentsecondary);
        document.querySelector('html').style.setProperty('--secondary-hover', localStorage.secondaryHoverColor);
        document.querySelector('html').style.setProperty('--secondary-border', localStorage.secondaryBorderColor);
        document.querySelector('body').classList.remove('dark-theme');
        document.querySelector('body').classList.remove('light-theme');
        document.querySelector('body').classList.add('transparent-theme');

        $('#myonoffswitch58').prop('checked', true);
    }
    if (localStorage.transparentcolor) {
        // document.getElementById('TransparentPrimaryColorID3').value = localStorage.transparentcolor;
        document.querySelector('html').style.setProperty('--transparent-color', localStorage.transparentcolor);
        document.querySelector('body').classList.remove('dark-theme');
        document.querySelector('body').classList.remove('light-theme');
        document.querySelector('body').classList.add('transparent-theme');

        $('#myonoffswitch58').prop('checked', true);
    }
}

// triggers on changing the color picker
function changePrimaryColor() {
    var userColor = document.getElementById('colorID').value;
    localStorage.setItem('primaryColor', userColor);
    // to store value as opacity 0.95 we use 95
    localStorage.setItem('primaryHoverColor', userColor + 95);
    localStorage.setItem('primaryBorderColor', userColor);
    localStorage.setItem('primaryTransparent', userColor + 20);

    // removing dark theme data
    localStorage.removeItem('darkPrimary')
    localStorage.removeItem('darksecondary')

    //removing transparent theme data
    localStorage.removeItem('transparentPrimary')
    localStorage.removeItem('transparentsecondary')
    localStorage.removeItem('transparentcolor')

    document.querySelector('body').classList.add('light-theme');
    document.querySelector('body').classList.remove('dark-theme');
    document.querySelector('body').classList.remove('transparent-theme');

    $('#myonoffswitch56').prop('checked', true);
    names()
}
function changeSecondaryColor() {
    var userColor = document.getElementById('colorID2').value;
    localStorage.setItem('secondaryColor', userColor);
    // to store value as opacity 0.95 we use 95
    localStorage.setItem('secondaryHoverColor', userColor + 95);
    localStorage.setItem('secondaryBorderColor', userColor);

    // removing dark theme data
    localStorage.removeItem('darkPrimary')
    localStorage.removeItem('darksecondary')

    //removing transparent theme data
    localStorage.removeItem('transparentPrimary')
    localStorage.removeItem('transparentsecondary')
    localStorage.removeItem('transparentcolor')

    document.querySelector('body').classList.add('light-theme');
    document.querySelector('body').classList.remove('dark-theme');
    document.querySelector('body').classList.remove('transparent-theme');

    $('#myonoffswitch56').prop('checked', true);
    names()
}

function darkPrimaryColor() {
    var userColor = document.getElementById('darkPrimaryColorID').value;
    localStorage.setItem('darkPrimary', userColor);
    // to store value as opacity 0.95 we use 95
    localStorage.setItem('primaryHoverColor', userColor + 95);
    localStorage.setItem('primaryBorderColor', userColor);

    // removing light theme data
    localStorage.removeItem('primaryColor')
    localStorage.removeItem('secondaryColor')

    //removing transparent theme data
    localStorage.removeItem('transparentPrimary')
    localStorage.removeItem('transparentsecondary')
    localStorage.removeItem('transparentcolor')

    document.querySelector('body').classList.add('dark-theme');
    document.querySelector('body').classList.remove('light-theme');
    document.querySelector('body').classList.remove('transparent-theme');

    $('#myonoffswitch57').prop('checked', true);
    names()
}

function changeDarkSecondaryColor() {
    var userColor = document.getElementById('darkColorID2').value;
    localStorage.setItem('darksecondary', userColor);
    // to store value as opacity 0.95 we use 95
    localStorage.setItem('secondaryHoverColor', userColor + 95);
    localStorage.setItem('secondaryBorderColor', userColor);

    // removing light theme data
    localStorage.removeItem('primaryColor')
    localStorage.removeItem('secondaryColor')

    //removing transparent theme data
    localStorage.removeItem('transparentPrimary')
    localStorage.removeItem('transparentsecondary')
    localStorage.removeItem('transparentcolor')

    document.querySelector('body').classList.add('dark-theme');
    document.querySelector('body').classList.remove('light-theme');
    document.querySelector('body').classList.remove('transparent-theme');

    $('#myonoffswitch57').prop('checked', true);
    names()
}

function TransparentPrimaryColor() {
    var userColor = document.getElementById('TransparentPrimaryColorID').value;
    localStorage.setItem('transparentPrimary', userColor);
    // to store value as opacity 0.95 we use 95
    localStorage.setItem('primaryHoverColor', userColor + 95);
    localStorage.setItem('primaryBorderColor', userColor);

    // removing light theme and dark theme data
    localStorage.removeItem('primaryColor')
    localStorage.removeItem('secondaryColor')
    localStorage.removeItem('darkPrimary')
    localStorage.removeItem('darksecondary')

    document.querySelector('body').classList.remove('dark-theme');
    document.querySelector('body').classList.remove('light-theme');
    document.querySelector('body').classList.add('transparent-theme');

    $('#myonoffswitch58').prop('checked', true);
    names()
}

function changeTransparentSecondaryColor() {
    var userColor = document.getElementById('TransparentColorID2').value;
    localStorage.setItem('transparentsecondary', userColor);
    // to store value as opacity 0.95 we use 95
    localStorage.setItem('secondaryHoverColor', userColor + 95);
    localStorage.setItem('secondaryBorderColor', userColor);

    // removing light theme data
    localStorage.removeItem('primaryColor')
    localStorage.removeItem('secondaryColor')
    localStorage.removeItem('darkPrimary')
    localStorage.removeItem('darksecondary')

    document.querySelector('body').classList.remove('dark-theme');
    document.querySelector('body').classList.remove('light-theme');
    document.querySelector('body').classList.add('transparent-theme');

    $('#myonoffswitch58').prop('checked', true);
    names()
}

function TransparentBackgroundColor() {
    var userColor = document.getElementById('TransparentPrimaryColorID3').value;
    localStorage.setItem('transparentcolor', userColor);

    // removing light theme data
    localStorage.removeItem('primaryColor')
    localStorage.removeItem('secondaryColor')
    localStorage.removeItem('darkPrimary')
    localStorage.removeItem('darksecondary')

    document.querySelector('body').classList.remove('dark-theme');
    document.querySelector('body').classList.remove('light-theme');
    document.querySelector('body').classList.add('transparent-theme');

    $('#myonoffswitch58').prop('checked', true);
    names()
}

// to check the value is hexa or not
const isValidHex = (hexValue) => /^#([A-Fa-f0-9]{3,4}){1,2}$/.test(hexValue)

const getChunksFromString = (st, chunkSize) => st.match(new RegExp(`.{${chunkSize}}`, "g"))
    // convert hex value to 256
const convertHexUnitTo256 = (hexStr) => parseInt(hexStr.repeat(2 / hexStr.length), 16)
    // get alpha value is equla to 1 if there was no value is asigned to alpha in function
const getAlphafloat = (a, alpha) => {
        if (typeof a !== "undefined") { return a / 255 }
        if ((typeof alpha != "number") || alpha < 0 || alpha > 1) {
            return 1
        }
        return alpha
    }
    // convertion of hex code to rgba code
function hexToRgba(hexValue, alpha) {
    if (!isValidHex(hexValue)) { return null }
    const chunkSize = Math.floor((hexValue.length - 1) / 3)
    const hexArr = getChunksFromString(hexValue.slice(1), chunkSize)
    const [r, g, b, a] = hexArr.map(convertHexUnitTo256)
    return `rgba(${r}, ${g}, ${b}, ${getAlphafloat(a, alpha)})`
}

let myVarVal, myVarVal1

function names() {
    let primaryColorVal = getComputedStyle(document.documentElement).getPropertyValue('--primary-bg-color').trim();
    let secondaryColorVal = getComputedStyle(document.documentElement).getPropertyValue('--secondary-color').trim();

    //get variable
    myVarVal = localStorage.getItem("primaryColor") || localStorage.getItem("darkPrimary") || localStorage.getItem("transparentPrimary") || primaryColorVal;
    myVarVal1 = localStorage.getItem("secondaryColor") || localStorage.getItem("darksecondary") || localStorage.getItem("transparentsecondary") || secondaryColorVal;

    if(document.querySelector('#sales-status') !== null){
        sales();
    }

    if(document.querySelector('#lineChart1') !== null){
        lineChart1();
    }

    if(document.querySelector('#survey') !== null){
        survey();
    }

    if(document.querySelector('#survey') !== null){
        morrisBar8();
    }

    if(document.querySelector('#purchase') !== null){
        purchase();
    }

    if(document.querySelector('#echart5') !== null){
        echart5();
    }

    if(document.querySelector('#echart1') !== null){
        echart1();
    }

    if(document.querySelector('#lineChart') !== null){
        lineChart();
    }

    let colorData4 = hexToRgba(myVarVal || primaryColorVal, 0.7)
    document.querySelector('html').style.setProperty('--primary-07', colorData4);

    let colorData5 = hexToRgba(myVarVal || primaryColorVal, 0.3)
    document.querySelector('html').style.setProperty('--primary-03', colorData5);

    let colorData6 = hexToRgba(myVarVal || primaryColorVal, 0.95)
    document.querySelector('html').style.setProperty('--primary-095', colorData6);

    let colorData7 = hexToRgba(myVarVal || primaryColorVal, 0.1)
    document.querySelector('html').style.setProperty('--primary-01', colorData7);

    let colorData8 = hexToRgba(myVarVal || primaryColorVal, 0.5)
    document.querySelector('html').style.setProperty('--primary-05', colorData8);

    let colorData11 = hexToRgba(myVarVal1 || secondaryColorVal, 0.1)
    document.querySelector('html').style.setProperty('--secondary-01', colorData11);

    let colorData12 = hexToRgba(myVarVal1 || secondaryColorVal, 0.7)
    document.querySelector('html').style.setProperty('--secondary-07', colorData12);

    let colorData13 = hexToRgba(myVarVal1 || secondaryColorVal, 0.5)
    document.querySelector('html').style.setProperty('--secondary-05', colorData13);
}
names()