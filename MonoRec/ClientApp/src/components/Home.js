import React, { useEffect, useState } from 'react';
import authService from './api-authorization/AuthorizeService'

function Home() {
    const displayName = Home.name;

    //delete everything below to keep original Home component
    const [PatientId, setPatientId] = useState('');
    const [Patients, setPatients] = useState([]);

    useEffect(() => {
        populatePatients();
    });

    const populatePatients = async () => {
        //const token = await authService.getAccessToken();
        //const user = await authService.getUser();
        const response = await fetch('patient', { method: 'GET' })

    //        , {
    //            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
    //}
        console.log(response);
        const data = await response.json();
        setPatients(data);
        console.log("this ran");
    }

    const handleSubmit = async (event) => {
        event.preventDefault();
        //const token = await authService.getAccessToken();
        const response = await fetch('patient',
            {
                method: 'POST',
                body: JSON.stringify({ PatientId }),
                headers: { 'Content-Type': 'application/json' }
            }
        );
    }

    return (
        <div>
            <h1>Hello, world!</h1>
            <p>Welcome to your new single-page application, built with:</p>
            <ul>
                <li><a href='https://get.asp.net/'>ASP.NET Core</a> and <a href='https://msdn.microsoft.com/en-us/library/67ef8sbd.aspx'>C#</a> for cross-platform server-side code</li>
                <li><a href='https://facebook.github.io/react/'>React</a> for client-side code</li>
                <li><a href='http://getbootstrap.com/'>Bootstrap</a> for layout and styling</li>
            </ul>
            <p>To help you get started, we have also set up:</p>
            <ul>
                <li><strong>Client-side navigation</strong>. For example, click <em>Counter</em> then <em>Back</em> to return here.</li>
                <li><strong>Development server integration</strong>. In development mode, the development server from <code>create-react-app</code> runs in the background automatically, so your client-side resources are dynamically built on demand and the page refreshes when you modify any file.</li>
                <li><strong>Efficient production builds</strong>. In production mode, development-time features are disabled, and your <code>dotnet publish</code> configuration produces minified, efficiently bundled JavaScript files.</li>
            </ul>
            <p>The <code>ClientApp</code> subdirectory is a standard React application based on the <code>create-react-app</code> template. If you open a command prompt in that directory, you can run <code>npm</code> commands such as <code>npm test</code> or <code>npm install</code>.</p>
        

            //delete everything below for original home component
            <h1>Create Visit Entry</h1>
            <div className="row">
                <div className="col-mid-6">
                    <form id="statsForm" onSubmit={handleSubmit}>
                        <select>
                            <option value="⬇️ Select a patient ⬇️"> -- Select a patient -- </option>
                            {Patients.map((patient) => <option value={patient.patientId}>{patient.patientName} | {patient.patientId}</option>)}
                        </select>
                        <div className="form-group">
                            <button type="submit" className="btn btn-success">Submit</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>

    );
}

export default Home;