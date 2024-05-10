import WeatherForecast from "../Components/WeatherForecast.tsx";
import AuthorizeView, {AuthorizedUser} from "../Components/AuthorizeView.tsx";
import LogoutLink from "../Components/LogoutLink.tsx";


function Home() {
    return (
        <AuthorizeView>
            <span><LogoutLink>Logout <AuthorizedUser value="email"/></LogoutLink></span>
            <WeatherForecast/>
        </AuthorizeView>
    )
}

export default Home;