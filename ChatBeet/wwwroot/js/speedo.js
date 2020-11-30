class Speedometer {
    binder;
    displayMode = 'car';
    defaultMax = 15;
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
                .then(num => {
                    this.rate = num;
                    if (num > this.max)
                        this.max = num;
                    this.binder.updateOutwardBindings();
                });
    }
    interval = null;
    initialize = () => {
        this.max = this.defaultMax;
        this.binder = new GravyBinder(this, document.getElementById('speedo-root'));
        this.binder.updateBindings();
        this.interval = window.setInterval(this.updateCounts, 10000);
    };
    meetsThreshold = (threshold) => {
        var ratio = this.rate / this.defaultMax;
        return ratio >= threshold;
    }

    constructor() {
        this.initialize();
    }
}

var speedo = new Speedometer();