import './App.css';
import {BrowserRouter, Route, Routes} from "react-router-dom";
import Home from "./Pages/Home.tsx";
import Register from "./Pages/Register.tsx";
import Login from "./Pages/Login.tsx";
//import { OAuth, useAuth } from './Components/OAuth.tsx';
import { ApplicationConstants } from './Utils/ApplicationConstants.ts';



function App(){
    //const oauthContext  = useAuth();


    return (
        <BrowserRouter>
            <Routes>
                <Route path={ApplicationConstants.Routes.Login} element={<Login/>} />
                <Route path={ApplicationConstants.Routes.Register} element={<Register/>} />
                <Route path={ApplicationConstants.Routes.Root} element={<Home/>} />
                {/* <Route path="/oauth" element={<OAuth/> } />
                <Route path='/login' Component={() => { oauthContext.login(); return null }} />
                <Route path='/logout' Component={() => { oauthContext.logout(); return null }}></Route> */}
        
            </Routes>
        </BrowserRouter>
    )
}

export default App;