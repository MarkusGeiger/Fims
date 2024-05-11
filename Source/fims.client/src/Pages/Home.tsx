import WeatherForecast from "../Components/WeatherForecast.tsx";
import AuthorizeView, {AuthorizedUser} from "../Components/AuthorizeView.tsx";
import LogoutLink from "../Components/LogoutLink.tsx";
import UserManagement from "../Components/UserManagement.tsx";


function Home() {
    return (
        <AuthorizeView>
            <span><LogoutLink>Logout <AuthorizedUser value="email"/></LogoutLink></span>
            <WeatherForecast/>
            <UserManagement/>
        </AuthorizeView>
    )
}

export default Home;