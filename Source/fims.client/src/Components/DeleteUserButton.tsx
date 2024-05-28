
function DeleteUserButton(props: {user: User, disabled: boolean}){

  async function handleDeleteClick(_e: React.MouseEvent) {
    console.log("User to be deleted: ", props.user.id);
    const confirmed = confirm(`Do you really want to delete user '${props.user.id}' (${props.user.username})`);
    if (confirmed) {
      await deleteUser(props.user.id);
    }
  }

  async function deleteUser(userId: string) {
    const response = await fetch(`api/user?id=${userId}`, { method: "DELETE" })
    console.log("User delete response:", response);

    if (!response.ok) {
      const data = await response.json();
      console.error("Failed to delete user.", data);
    }
    else {
      //await getUserData();
    }
  }

  return <button disabled={props.disabled} onClick={handleDeleteClick}>delete</button>
}

export default DeleteUserButton;