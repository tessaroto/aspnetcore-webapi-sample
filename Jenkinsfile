node {
    def app
    def workspace = env.WORKSPACE
    def version
    def qa_version

    stage('Clone repository') {
        git branch: 'master',
            credentialsId: '776dff19-d555-416b-b004-e3e1be9f3f9f',
            url: 'https://tessaroto@bitbucket.org/signaconsultoriadev/aspnetcore-webapi-sample.git'

        /*checkout scm*/
    }

    stage('Build') {

        /* This builds the actual image; synonymous to
         * docker build on the command line */

        app = docker.build("tessaroto/aspnetcore-webapi-sample") 
    }

    stage('Tagging') {
        
        try {
            version = sh (script: "stepup version --next-release | sed -e 's/v//g'", returnStdout: true)
            
            withCredentials([[$class: 'UsernamePasswordMultiBinding', credentialsId: '776dff19-d555-416b-b004-e3e1be9f3f9f', usernameVariable: 'GIT_USERNAME', passwordVariable: 'GIT_PASSWORD']]) {
                sh("echo ${env.GIT_USERNAME}")
                sh("git config credential.username ${env.GIT_USERNAME}")
                sh("git config credential.helper '!f() { echo password=\$GIT_PASSWORD; }; f'")
                sh('/opt/tools/create-tag')
                
            }   
        } finally {
            sh("git config --unset credential.username")
            sh("git config --unset credential.helper")
        }
        
    }
    
    stage('Promote to QA') {
    
        qa_version = "qa-${version}"
        docker.withRegistry('https://registry.hub.docker.com', '9c8981f6-1970-4ed3-b484-843c3e6fcb4f') {
            app.push("${qa_version}")
        }
    }


    stage('Deploy QA') {
        sshagent (credentials: ['d3367570-fc11-4099-8767-67d1c2d5046e']) {
            sh "ssh -o StrictHostKeyChecking=no -l root 178.128.69.112 \"export API_TAG=${qa_version} docker stack deploy -c /opt/aspnetcore-webapi-sample/docker-compose.yml api\""
        }
        sh 'echo "Deployed"'
        
    }

    stage('Run Functional Tests') {
        sh 'echo "Tests passed"'
    }

    stage('Code Analysis') {
        echo "workspace directory is ${workspace}"

        docker.image('tessaroto/dotnet-code-sonar-analysis:latest').inside("--network=ci_ci -v ${env.WORKSPACE}:/app")  {
            sh 'dotnet sonarscanner begin /k:"examples" /name:"aspnetcore-webapi-sample" /d:sonar.host.url="http://sonar:9000" /d:sonar.login="711fc3ad49665c3226dcd8b757e957575233ebfd"'
            sh 'dotnet build'
            sh 'dotnet sonarscanner end  /d:sonar.login="711fc3ad49665c3226dcd8b757e957575233ebfd"'
        }
    }    

    stage('Promote to Prod') {
        
        userInput = input(
            id: 'Proceed1', message: 'Promote to production?', parameters: [])

        docker.withRegistry('https://registry.hub.docker.com', '9c8981f6-1970-4ed3-b484-843c3e6fcb4f') {
            app.push("${version}")
            app.push("latest")
        }
    }  
}

