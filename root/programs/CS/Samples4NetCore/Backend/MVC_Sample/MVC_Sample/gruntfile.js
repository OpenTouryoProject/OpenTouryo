module.exports = function (grunt) {
    grunt.initConfig({
        // package.json�t�@�C�����̃v���p�e�B�̒l���Q�Ƃ���
        pkg: grunt.file.readJSON('package.json'),
        // �e�^�X�N�̍\�����`
        // package.json �� scripts ���� "grunt cssmin" �ȂǂŌĂяo����B
        copy: {
            bundle: {
                files: [
                    {
                        expand: true, src: ['**'],
                        cwd: 'node_modules/jquery/dist/',
                        dest: 'wwwroot/lib/jquery/dist/'
                    },
                    {
                        expand: true, src: ['**'],
                        cwd: 'node_modules/jquery-validation/dist/',
                        dest: 'wwwroot/lib/jquery-validation/dist/'
                    },
                    {
                        expand: true, src: ['**'],
                        cwd: 'node_modules/jquery-validation-unobtrusive/dist/', 
                        dest: 'wwwroot/lib/jquery-validation-unobtrusive/dist/'
                    },
                    {
                        expand: true, src: ['**'],
                        cwd: 'node_modules/bootstrap/dist/',
                        dest: 'wwwroot/lib/bootstrap/dist/'
                    },
                    {
                        expand: true, src: ['**'],
                        cwd: 'node_modules/font-awesome/css/',
                        dest: 'wwwroot/css/'
                    },
                    {
                        expand: true, src: ['**'],
                        cwd: 'node_modules/font-awesome/fonts/',
                        dest: 'wwwroot/fonts/'
                    }
                ]
            }
        },
        cssmin: {
            bundle: {
                src: [
                    'wwwroot/css/app/site.css',
                    'wwwroot/css/touryo/Style.css',
                    'wwwroot/lib/bootstrap/dist/css/bootstrap.css',
                    'wwwroot/css/font-awesome.css'
                ],
                dest: 'wwwroot/css/css.min.css'
            }
        },
        uglify: {
            bundle: {
                files: {
                    'wwwroot/js/header.min.js': [
                        'wwwroot/lib/jquery/dist/jquery.js',
                        'wwwroot/lib/bootstrap/dist/js/bootstrap.js'],
                    'wwwroot/js/footer.min.js': [
                        'wwwroot/lib/jquery-validation/dist/jquery.validate.js',
                        'wwwroot/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.js'],
                    'wwwroot/js/app.min.js': [
                        'wwwroot/js/app/site.js'
                    ],
                    'wwwroot/js/touryo.min.js': [
                        'wwwroot/js/touryo/common.js',
                        'wwwroot/js/touryo/else.js']
                }
            }
        }
    });
    // �K�v��Grunt�v���O�C����ǂݍ��ށB
    grunt.loadNpmTasks('grunt-contrib-cssmin');
    grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadNpmTasks('grunt-contrib-copy');
    grunt.loadNpmTasks('grunt-contrib-cssmin');
    grunt.loadNpmTasks('grunt-contrib-uglify-es');
    // �e�^�X�N��g�������J�X�^�� �^�X�N��o�^����B
    // package.json �� scripts ���� "grunt bundle" �ŌĂяo����B
    grunt.registerTask('bundle', ['copy', 'cssmin', 'uglify']);
};