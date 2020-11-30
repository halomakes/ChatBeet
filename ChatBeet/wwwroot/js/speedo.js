class Speedometer {
    binder;
    displayMode = 'dial';
    defaultMax = 40;
    max;
    rate = 0;
    channelName = '';
    setDisplayMode = (newMode) => {
        this.displayMode = newMode;
        this.binder.updateOutwardBindings();
    };
    updateCounts = () => {
        if (this.channelName)
            fetch(`/api/speedometer/${this.channelName.replace('#', '')}`)
                .then(r => r.json())
                .then(this.setValue);
    }
    interval = null;
    initialize = () => {
        this.max = this.defaultMax;
        this.binder = new GravyBinder(this, document.getElementById('speedo-root'));
        this.binder.updateBindings();
        this.interval = window.setInterval(this.updateCounts, 5000);

        this.gauge.setMinValue(0);  // Prefer setter over gauge.minValue = 0
    };
    meetsThreshold = (threshold) => {
        var ratio = this.rate / this.defaultMax;
        return ratio >= threshold;
    };
    gauge = new Gauge(document.getElementById('car-speedo')).setOptions({
        angle: 0.15,
        lineWidth: 0.44,
        radiusScale: 1,
        pointer: {
            length: 0.6,
            strokeWidth: 0.09,
            color: '#888888'
        },
        limitMax: false,
        colorStart: '#3395ff',
        colorStop: '#3395ff',
        strokeColor: '#E0E0E0',
        generateGradient: true,
        highDpiSupport: true,
    });
    setValue = num => {
        this.rate = num;
        if (num > this.max)
            this.max = num;
        this.binder.updateOutwardBindings();

        if (this.gauge.maxValue !== this.max)
            this.gauge.maxValue = this.max;
        this.gauge.set(this.rate);
    }

    constructor() {
        this.initialize();
    }
}

var speedo = new Speedometer();