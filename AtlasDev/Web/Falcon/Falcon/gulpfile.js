var gulp = require('gulp'),
    gutil = require('gulp-util'),
    uglify = require('gulp-uglify'),
    jshint = require('gulp-jshint'),
    concat = require('gulp-concat'),
    clean = require('gulp-clean'),
    jshintreporter = require('jshint-stylish'),
    minifycss = require('gulp-minify-css'),
    size = require('gulp-size'),
    clean = require('gulp-clean'),
    rename = require('gulp-rename'),
    gzip = require('gulp-gzip');

var paths = {
    appjslint: {
        src: './app/**/*.js',
        dest: './build/'
    },
    appjsconcact: {
        src: [
            './assets/plugins/jquery-2.1.1.min.js',
            './assets/plugins/jquery-validation/dist/jquery.validate.min.js',
            './assets/plugins/jquery-slimscroll/jquery.slimscroll.min.js',
            './assets/plugins/jquery.blockui.min.js',
            './assets/plugins/jquery.cokie.min.js',
            './assets/plugins/jquery-inputmask/jquery.inputmask.bundle.min.js',
            './assets/plugins/bootstrap-select/bootstrap-select.min.js',
            './assets/plugins/select2/select2.min.js',
            './assets/plugins/jquery-price-format/jquery.price_format.2.0.min.js',
            './assets/plugins/bootstrap/js/bootstrap.min.js',
            './assets/plugins/bootstrap-hover-dropdown/bootstrap-hover-dropdown.min.js',
            './assets/plugins/jquery.pulsate.min.js',
            './assets/plugins/uniform/jquery.uniform.min.js',
            './assets/scripts/xsockets/XSockets.latest.min.js',
            './assets/scripts/moment.min.js',
            './assets/scripts/angular/1.3/min/angular.min.js',
            './assets/scripts/angular/1.3/min/angular-route.min.js',
            './assets/scripts/angular/1.3/min/angular-resource.min.js',
            './assets/scripts/angular/1.3/min/angular-cookies.min.js',
            './assets/scripts/angular/1.3/min/angular-sanitize.min.js',
            './assets/scripts/angular/1.3/min/angular-animate.min.js',
            './assets/scripts/angular/1.3/plugins/ngRemoteValidate.0.4.1.min.js',
            './assets/scripts/angular/1.3/plugins/angular-toastr.min.js',
            './assets/scripts/angular/1.3/plugins/angular-moment.min.js',
            './assets/scripts/angular/1.3/plugins/ui-grid.min.js',
            './assets/scripts/ngprogress.min.js'
        ],
        dest: './build/'

    },
    appjsmin: {
        src: [

            './assets/plugins/jquery-idle-timeout/jquery.idletimeout.js',
            './assets/plugins/jquery-idle-timeout/jquery.idletimer.js',
            './assets/plugins/jquery-multi-select/js/jquery.multi-select.js',
            './assets/plugins/bootstrap-datepicker/js/bootstrap-datepicker.js',
            './assets/scripts/components-form-tools.js',
            './assets/scripts/components-pickers.js',
            './assets/scripts/components-jqueryui-sliders.js',
            './assets/scripts/ui-idletimeout.js',
            './assets/scripts/metronic.js',
            './assets/scripts/layout.js',
            './assets/scripts/quick-sidebar.js',
            './assets/scripts/platform.js',
            './assets/scripts/json2.js',
            './assets/scripts/linq/linq.js',
            './assets/scripts/linq/linq.jquery.js',
            './assets/scripts/custom/tab-restore.js',
            './assets/scripts/custom/init.js',
            './assets/scripts/index.js',
            './assets/scripts/login.js',
            './assets/scripts/custom.js',
            './assets/scripts/angular/1.3/i18n/angular-locale_en-za.js',
            './assets/scripts/angular/1.3/plugins/ng-table.js',
            './assets/scripts/angular/1.3/plugins/ng-tags-input.js',
            './assets/scripts/angular-multi.select/js/angular-multi-select.js',
            './assets/plugins/jquery.globalize/globalize.js',
            './assets/plugins/jquery.globalize/cultures/globalize.culture.en-ZA.js',
            './assets/plugins/devexpress/dx.all.js',
            './assets/plugins/filesaver/FileSaver.js',
            './assets/plugins/devexpress/dx.chartjs.js',
            './assets/plugins/devexpress/dx.exporter.js',
            './assets/plugins/devexpress/dx.webappjs.js'
        ],
        dest: './build/'
    },
    csspath: {
        src: [
            './assets/plugins/font-awesome/css/font-awesome.min.css',
            './assets/plugins/bootstrap/css/bootstrap.min.css',
            './assets/plugins/bootstrap-select/bootstrap-select.min.css',
            './assets/plugins/jquery-multi-select/css/multi-select.css',
            './assets/plugins/select2/select2.css',
            './assets/plugins/simple-line-icons/simple-line-icons.min.css',
            './assets/plugins/uniform/css/uniform.default.css',
            './assets/css/ng-tags-input.css',
            './assets/css/ng-tags-input.bootstrap.css',
            './assets/css/animate.css',
            './assets/css/components.css',
            './assets/css/plugins.css',
            './assets/css/layout.css',
            './assets/css/themes/red-sunglo.css',
            './assets/css/toaster.css',
            './assets/css/ng-table.css',
            './assets/css/custom.css',
            './assets/css/devexpress/dx.common.css',
            './assets/css/devexpress/dx.dark.css',
            './assets/css/devexpress/dx.light.css',
            './assets/scripts/angular-multi.select/css/angular-multi-select.css',
            './assets/plugins/bootstrap-datepicker/css/datepicker3.css',
            './assets/css/ui-grid.min.css'
        ],
        dest: './build/'
    }
};

gulp.task('app-css-minify', function() {
    gulp.src(paths.csspath.src)
        .pipe(concat('app.css'))
        .pipe(minifycss())
        .pipe(gzip())
        .pipe(rename({
            suffix: '.min'
        }))
        .pipe(gulp.dest(paths.csspath.dest))
});

gulp.task('app-js-concat-minified', function() {
    gulp.src(paths.appjsconcact.src)
        .pipe(concat('app.minified.js'))
        .pipe(size())
        .pipe(gulp.dest(paths.appjsconcact.dest))
});

gulp.task('app-core-minify', function() {
    gulp.src(paths.appjsmin.src)
        .pipe(uglify())
        .pipe(concat('app.core.js'))
        .pipe(size())
        .pipe(rename({
            suffix: '.min'
        }))
        .pipe(gulp.dest(paths.appjsmin.dest))
});

gulp.task('app-minify', function() {
    gulp.src(paths.appjslint.src)
        .pipe(uglify())
        .pipe(concat('app.js'))
        .pipe(size())
        .pipe(rename({
            suffix: '.min'
        }))
        .pipe(gulp.dest(paths.appjslint.dest))
});

gulp.task('app-js-hint', function() {
    gulp.src(paths.appjslint.src)
        .pipe(jshint())
        .pipe(jshint.reporter(jshintreporter));

});

gulp.task('app-clean-build', function() {
    gulp.src('./build', {
        read: false
    })
        .pipe(clean());

});

var tasksToRun = [];

if (process.env.NODE_ENV === 'Release') {
    tasksToRun.push('app-js-concat-minified');
    tasksToRun.push('app-core-minify');
    tasksToRun.push('app-minify');
    tasksToRun.push('app-css-minify');
} else {
    tasksToRun.push('app-js-hint');
}

gulp.task('default', tasksToRun);
gulp.task('clean', ['app-clean-build']);
gulp.task('build', ['app-js-concat-minified','app-core-minify', 'app-minify', 'app-css-minify']);