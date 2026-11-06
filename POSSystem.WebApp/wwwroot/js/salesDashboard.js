window.salesDashboard = {
    observer: null,

    initialize: function (dotNetHelper) {
        const target = document.getElementById("sales-summary");
        if (!target) return;

        this.observer = new IntersectionObserver(entries => {
            const isVisible = entries[0].isIntersecting;

            dotNetHelper.invokeMethodAsync(
                "UpdateFloatingSummary",
                !isVisible
            );
        });

        this.observer.observe(target);
    },

    dispose: function () {
        if (this.observer) {
            this.observer.disconnect();
            this.observer = null;
        }
    }
};